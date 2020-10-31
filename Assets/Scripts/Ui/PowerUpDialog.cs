using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ui {
  public class PowerUpDialog : MonoBehaviour {

    private const string IS_SHOW = "IsShow";

    public enum PowerUpAbility {
      UNKNOWN = 0,
      RELAY = 1,
      TIMESLOW = 2,
      HEART = 3,
    }


    // Intro playable:
    // DO: 1) Show dialog; 2) Trigger StartWaitingForPlayerConfirm().
    // DON'T: 1) Disable player control, which has been done by this component.
    public PlayableDirector intro;

    // Outro playable:
    // DO: 1) Hide dialog; 2) Grant powerful. 
    // DON'T: 1) Enable player control, which has been done by this component.
    public PlayableDirector outro;
    public bool usePowerUpDialogV2 = false;
    public PowerUpAbility powerUpAbility;

    private bool hasTriggerred = false;
    private bool isWaitingForPlayerConfirm = false;

    private DeviceInput playerDeviceInput;
    private InputMaster inputMaster;
    private Animator animator;

    private void Awake() {
      inputMaster = new InputMaster();
      inputMaster.General.Shoot.performed += _ => {
        Debug.Log("Interactive button click.");
        if (isWaitingForPlayerConfirm) {
          isWaitingForPlayerConfirm = false;
          StartOutro();
        }
      };

      animator = GetComponent<Animator>();
      playerDeviceInput = GameObject.FindGameObjectWithTag(Tags.PLAYER)?.GetComponent<DeviceInput>();
      Debug.Assert(playerDeviceInput != null, "Cannot find Player GameObject or DeviceInput in it.");

    }

    private void OnTriggerEnter2D(Collider2D collision) {
      if (hasTriggerred || !collision.gameObject.CompareTag(Tags.PLAYER)) {
        return;
      }

      // Start
      hasTriggerred = true;
      DisablePlayerControl();
      GetComponent<SpriteRenderer>().enabled = false;
      // Show dialog and trigger StartWaitingForPlayerConfirm().
      StartIntro();
    }

    // Triggered by animation/playable
    public void StartWaitingForPlayerConfirm() {
      isWaitingForPlayerConfirm = true;
      applyPowerUp();
    }

    private void applyPowerUp() {
      switch (powerUpAbility) {
        case PowerUpAbility.HEART:
        MaxHealthPlus();
        break;
        case PowerUpAbility.RELAY:
        EnableRelay();
        break;
        case PowerUpAbility.TIMESLOW:
        EnableTimeslow();
        break;
        default:
        Debug.LogErrorFormat("Undefined power up ability: {0}", powerUpAbility);
        break;
      }
    }

    public void DisablePlayerControl() {
      playerDeviceInput.enabled = false;
    }

    public void EnablePlayerControl() {
      playerDeviceInput.enabled = true;
    }

    public void MaxHealthPlus() {
      GameObject playerGo = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      Debug.Assert(playerGo != null, "Cannot find Player GO by tag.");
      PlayerController playerController = playerGo.GetComponent<PlayerController>();
      Debug.Assert(playerController != null, "Cannot find PlayerController in player GO.");
      playerController.maxHealth += 1;
      playerController.currentHealth = playerController.maxHealth;
    }

    private void EnableRelay() {
      PlayerController playerController = GameObject.FindGameObjectWithTag(Tags.PLAYER)?.GetComponent<PlayerController>();
      playerController.supportRelay = true;
    }

    private void EnableTimeslow() {
      PlayerController playerController = GameObject.FindGameObjectWithTag(Tags.PLAYER)?.GetComponent<PlayerController>();
      playerController.supportTimeslow = true;
    }

    private void StartIntro() {
      if (usePowerUpDialogV2) {
        animator.SetBool(IS_SHOW, true);
      } else {
        Debug.AssertFormat(intro != null, "Intro PlayerDirector is null on PowerUp {0}.", gameObject.name);
        intro.Play();
      }
    }

    private void StartOutro() {
      if (usePowerUpDialogV2) {
        animator.SetBool(IS_SHOW, false);
      } else {
        Debug.AssertFormat(outro != null, "Outro PlayerDirector is null on PowerUp {0}.", gameObject.name);
        outro.Play();
      }
      EnablePlayerControl();
    }

    private void OnEnable() {
      inputMaster.Enable();
    }

    private void OnDisable() {
      inputMaster.Disable();
    }
  }
}

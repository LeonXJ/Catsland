using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ui {
  public class PowerUpDialog : MonoBehaviour {

    // Intro playable:
    // DO: 1) Show dialog; 2) Trigger StartWaitingForPlayerConfirm().
    // DON'T: 1) Disable player control, which has been done by this component.
    public PlayableDirector intro;

    // Outro playable:
    // DO: 1) Hide dialog; 2) Grant powerful. 
    // DON'T: 1) Enable player control, which has been done by this component.
    public PlayableDirector outro;

    private bool hasTriggerred = false;
    private bool isWaitingForPlayerConfirm = false;

    private DeviceInput playerDeviceInput;
    private InputMaster inputMaster;

    private void Awake() {
      inputMaster = new InputMaster();
      inputMaster.General.Shoot.performed += _ => {
        Debug.Log("Interactive button click.");
        if (isWaitingForPlayerConfirm) {
          isWaitingForPlayerConfirm = false;
          StartOutro();
        }
      };

      playerDeviceInput = GameObject.FindGameObjectWithTag(Common.Tags.PLAYER)?.GetComponent<DeviceInput>();
      Debug.Assert(playerDeviceInput != null, "Cannot find Player GameObject or DeviceInput in it.");
    }

    private void OnTriggerEnter2D(Collider2D collision) {
      if (hasTriggerred || !collision.gameObject.CompareTag(Common.Tags.PLAYER)) {
        return;
      }

      // Start
      hasTriggerred = true;
      DisablePlayerControl();
      // Show dialog and trigger StartWaitingForPlayerConfirm().
      StartIntro();
    }

    public void StartWaitingForPlayerConfirm() {
      isWaitingForPlayerConfirm = true;
    }

    public void DisablePlayerControl() {
      playerDeviceInput.enabled = false;
    }

    public void EnablePlayerControl() {
      playerDeviceInput.enabled = true;
    }

    private void StartIntro() {
      Debug.AssertFormat(intro != null, "Intro PlayerDirector is null on PowerUp {0}.", gameObject.name);
      intro.Play();
    }

    private void StartOutro() {
      Debug.AssertFormat(outro != null, "Outro PlayerDirector is null on PowerUp {0}.", gameObject.name);
      outro.Play();
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

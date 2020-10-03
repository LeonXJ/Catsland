using UnityEngine;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Misc {

  [RequireComponent(typeof(Collider2D))]
  public class Checkpoint : MonoBehaviour {
    private const string IS_LIT = "IsLit";

    public bool lightOnWake = false;
    private bool isInInteractionRange = false;

    private Animator animator;
    private InputMaster inputMaster;
    private AudioSource audioSource;

    private void Awake() {
      animator = GetComponent<Animator>();
      inputMaster = new InputMaster();
      inputMaster.General.Interact.performed += _ => {
        if (canLit) {
          lit();
          Save();
        }
      };
      audioSource = GetComponent<AudioSource>();
    }

    void Start() {
      if (lightOnWake) {
        lit();
      }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
      if (collision.gameObject.CompareTag(Tags.PLAYER)) {
        isInInteractionRange = true;
      }
    }
    private void OnTriggerStay2D(Collider2D collision) {
      if (collision.gameObject.CompareTag(Tags.PLAYER)) {
        isInInteractionRange = true;
      }
    }
    private void OnTriggerExit2D(Collider2D collision) {
      if (collision.gameObject.CompareTag(Tags.PLAYER)) {
        isInInteractionRange = false;
      }
    }

    private bool canLit => isInInteractionRange && !lightOnWake;

    public void lit() {
      animator?.SetBool(IS_LIT, true);
    }

    public void ContinuePlayCampFireSound() {
      if (!audioSource.isPlaying) {
        audioSource.Play();
      }
    }

    public void StopPlayCampFireSound() {
      if (audioSource.isPlaying) {
        audioSource.Stop();
      }
    }

    private void Save() {
      Debug.Log("Progress Saved.");
      GameObject playerGo = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      Debug.Assert(playerGo != null, "Can't find Player GO.");
      SceneConfig.getSceneConfig().getProgressManager().Save(
        ProgressManager.Progress.Create(playerGo));
    }

    public void unlit() {
      animator?.SetBool(IS_LIT, false);
    }
    private void OnEnable() {
      inputMaster.Enable();
    }

    private void OnDisable() {
      inputMaster.Disable();
    }
  }
}



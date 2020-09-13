using UnityEngine;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Misc {
  public class SceneTransitionTrigger : MonoBehaviour {

    public SceneMaster.SceneTransitInfo transiteInfo;
    public bool needInputConfirm = false;

    private InputMaster inputMaster;
    private bool hasTransitionTriggerred = false;
    private bool isTriggerred = false;

    private void Awake() {
      inputMaster = new InputMaster();
      inputMaster.General.SceneTransitConfirm.performed += SceneTransitConfirm_performed;
    }

    private void SceneTransitConfirm_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
      if (isTriggerred && !hasTransitionTriggerred && needInputConfirm) {
        hasTransitionTriggerred = true;
        SceneMaster.getInstance().TransitionToScene(transiteInfo);
      }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
      process(collision);
    }
    private void OnTriggerStay2D(Collider2D collision) {
      process(collision);
    }

    private void OnTriggerExit2D(Collider2D collision) {
      if (collision.gameObject.CompareTag(Common.Tags.PLAYER)) {
        isTriggerred = false;
      }
    }

    private void process(Collider2D collider) {
      if (collider.gameObject.CompareTag(Common.Tags.PLAYER)) {
        isTriggerred = true;
        if (needInputConfirm || hasTransitionTriggerred) {
          return;
        }
        hasTransitionTriggerred = true;
        SceneMaster.getInstance().TransitionToScene(transiteInfo);
      }
    }

    private void OnEnable() {
      inputMaster.Enable();
    }

    private void OnDisable() {
      inputMaster.Disable();
    }
  }
}

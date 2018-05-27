using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class TriggerBasedSensor :MonoBehaviour, ISensor {

    public LayerMask layerMask;

    private bool isTriggered = false;
    private GameObject triggerGO;

    public void OnTriggerEnter2D(Collider2D collision) {
      onTriggerEnterOrStay(collision);
    }

    public void OnTriggerStay2D(Collider2D collision) {
      onTriggerEnterOrStay(collision);
    }

    public void OnTriggerExit2D(Collider2D collision) {
      if(isCollisionWhitelisted(collision)) {
        isTriggered = false;
        triggerGO = null;
      }
    }

    public bool isStay() {
      return isTriggered;
    }

    public GameObject getTriggerGO() {
      return triggerGO;
    }

    private bool isCollisionWhitelisted(Collider2D collision) {
      return (layerMask & (1 << collision.gameObject.layer)) != 0x0;
    }

    private void onTriggerEnterOrStay(Collider2D collision) {
      if(isCollisionWhitelisted(collision)) {
        isTriggered = true;
        triggerGO = collision.gameObject;
      }
    }
  }
}

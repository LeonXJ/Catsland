using UnityEngine;

namespace Catsland.Scripts.CharacterController {
  public class TriggerBasedSensor :MonoBehaviour, ISensor {

    public LayerMask layerMask;

    private bool isTriggered = false;

    public void OnTriggerEnter2D(Collider2D collision) {
      isTriggered = isCollisionWhitelisted(collision);
    }

    public void OnTriggerStay2D(Collider2D collision) {
      isTriggered = isCollisionWhitelisted(collision);
    }

    public void OnTriggerExit2D(Collider2D collision) {
      isTriggered = !isCollisionWhitelisted(collision);
    }

    public bool isStay() {
      return isTriggered;
    }

    private bool isCollisionWhitelisted(Collider2D collision) {
      return (layerMask & (1 << collision.gameObject.layer)) != 0x0;
    }
  }
}

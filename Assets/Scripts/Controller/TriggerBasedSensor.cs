using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class TriggerBasedSensor :MonoBehaviour, ISensor {

    public LayerMask layerMask;
    public List<GameObject> whitelistGos;

    public bool isTriggered = false;
    public GameObject triggerGO;
    public HashSet<GameObject> triggerGos = new HashSet<GameObject>();

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
        triggerGos.Remove(collision.gameObject);
      }
    }

    public bool isStay() {
      if(triggerGO == null) {
        isTriggered = false;
      }
      //return isTriggered;
      return triggerGos.Count > 0;
    }

    public HashSet<GameObject> getTriggerGos() {
      return triggerGos;
    }

    private bool isCollisionWhitelisted(Collider2D collision) {
      bool masked = (layerMask & (1 << collision.gameObject.layer)) == 0x0;
      if(masked) {
        return false;
      }
      if(whitelistGos != null && whitelistGos.Count > 0) {
        return whitelistGos.Contains(collision.gameObject);
      }
      return true;
    }

    private void onTriggerEnterOrStay(Collider2D collision) {
      if(isCollisionWhitelisted(collision)) {
        isTriggered = true;
        triggerGO = collision.gameObject;
        triggerGos.Add(collision.gameObject);
      }
    }
  }
}

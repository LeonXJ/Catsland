using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class TriggerBasedSensor: MonoBehaviour, ISensor {

    public LayerMask layerMask;
    public List<GameObject> whitelistGos;

    public HashSet<GameObject> triggerGos = new HashSet<GameObject>();
    public bool debug = false;

    public void OnTriggerEnter2D(Collider2D collision) {
      onTriggerEnterOrStay(collision);
    }

    public void OnTriggerStay2D(Collider2D collision) {
      onTriggerEnterOrStay(collision);
    }

    public void OnTriggerExit2D(Collider2D collision) {
      if(isCollisionWhitelisted(collision)) {
        triggerGos.Remove(collision.gameObject);
      }
    }

    public bool isStay() {
      cleanUpDeleteGos();
      return triggerGos.Count > 0;
    }

    public HashSet<GameObject> getTriggerGos() {
      cleanUpDeleteGos();
      return triggerGos;
    }

    void cleanUpDeleteGos() {
      triggerGos.RemoveWhere(gameObject => gameObject == null);
    }

    private bool isCollisionWhitelisted(Collider2D collision) {
      bool masked = (layerMask & (1 << collision.gameObject.layer)) == 0x0;
      if(masked) {
        return false;
      }
      if(whitelistGos != null && whitelistGos.Count > 0) {
        if(!whitelistGos.Contains(collision.gameObject)) {
          Debug.Log("Whitelist filter: " + collision.name);
        }
        return whitelistGos.Contains(collision.gameObject);
      }
      return true;
    }

    private void onTriggerEnterOrStay(Collider2D collision) {
      if(isCollisionWhitelisted(collision)) {
        if(debug) {
          Debug.Log("Collide: " + collision.name);
        }
        triggerGos.Add(collision.gameObject);
      }
    }
  }
}

using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Bullets {
  public class Melee :MonoBehaviour {

    public LayerMask layerMask;
    public List<GameObject> muteGameObjects;

    private DamageInfo damageInfo = new DamageInfo(1, Vector2.right, 1.0f);
    private HashSet<GameObject> processedGameObjects = new HashSet<GameObject>();

    private Collider2D collider2d;

    void Start() {
      collider2d = gameObject.GetComponent<Collider2D>();
      collider2d.enabled = false;
    }

    public void OnTriggerStay2D(Collider2D collision) {
      if(!isCollisionWhitelisted(collision)) {
        return;
      }
      if(muteGameObjects.Contains(collision.gameObject)) {
        return;
      }
      if(processedGameObjects.Contains(collision.gameObject)) {
        return;
      }

      // make damage
      processedGameObjects.Add(collision.gameObject);
      collision.gameObject.SendMessage(
        MessageNames.DAMAGE_FUNCTION,
        damageInfo,
        SendMessageOptions.DontRequireReceiver);
    }

    public void turnOn(DamageInfo damageInfo) {
      processedGameObjects.Clear();
      this.damageInfo = damageInfo;
      collider2d.enabled = true;
    }

    public void turnOff() {
      collider2d.enabled = false;
    }

    private bool isCollisionWhitelisted(Collider2D collision) {
      return (layerMask & (1 << collision.gameObject.layer)) != 0x0;
    }
  }
}

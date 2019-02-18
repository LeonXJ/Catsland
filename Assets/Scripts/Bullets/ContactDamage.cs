using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Bullets {
  public class ContactDamage: MonoBehaviour {
    public int damage;
    public float repelIntensity;
    public GameObject owner;

    void OnCollisionEnter2D(Collision2D collision) {
      onHit(collision);
    }

    void OnTriggerEnter2D(Collider2D collider) {
      onHit(collider);
    }

    void OnCollisionStay2D(Collision2D collision) {
      onHit(collision);
    }

    void onHit(Collision2D collision) {
      onHitGameObject(collision.collider);
    }

    void onHit(Collider2D collider) {
      onHitGameObject(collider);
    }

    private void onHitGameObject(Collider2D collider) {
      if(!enabled) {
        return;
      }
      GameObject collidingGameObject = collider.gameObject;
      if(collidingGameObject.layer == Layers.LayerCharacter && collidingGameObject != owner) {
        Debug.Log("Real Hit " + collidingGameObject.name);
        Vector2 delta = collidingGameObject.transform.position - transform.position;
        collidingGameObject.SendMessage(
          MessageNames.DAMAGE_FUNCTION,
          new DamageInfo(
            damage, collider.bounds.center, new Vector2(Mathf.Sign(delta.x), 0.0f), repelIntensity),
          SendMessageOptions.DontRequireReceiver);
      }
    }
  }
}

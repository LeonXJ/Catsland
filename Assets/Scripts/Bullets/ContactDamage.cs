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
      onHitGameObject(collision.gameObject);
    }

    void onHit(Collider2D collider) {
      onHitGameObject(collider.gameObject);
    }

    private void onHitGameObject(GameObject gameObject) {
      if(!enabled) {
        return;
      }
      if(gameObject.layer == Layers.LayerCharacter && gameObject != owner) {
        Vector2 delta = gameObject.transform.position - transform.position;
        gameObject.SendMessage(
          MessageNames.DAMAGE_FUNCTION,
          new DamageInfo(damage, new Vector2(Mathf.Sign(delta.x), 0.0f), repelIntensity),
          SendMessageOptions.DontRequireReceiver);
      }
    }
  }
}

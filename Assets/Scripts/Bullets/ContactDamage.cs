using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Bullets {
  public class ContactDamage: MonoBehaviour {

    public int damage;
    public float repelIntensity;

    void OnCollisionEnter2D(Collision2D collision) {
      onHit(collision);
    }

    void OnCollisionStay2D(Collision2D collision) {
      onHit(collision);
    }

    void onHit(Collision2D collision) {
      if(collision.gameObject.layer == Layers.LayerCharacter) {
        Vector2 delta = collision.gameObject.transform.position - transform.position;
        collision.gameObject.SendMessage(
          MessageNames.DAMAGE_FUNCTION,
          new DamageInfo(damage, new Vector2(Mathf.Sign(delta.x), 0.0f), repelIntensity),
          SendMessageOptions.DontRequireReceiver);
      }
    }
  }
}

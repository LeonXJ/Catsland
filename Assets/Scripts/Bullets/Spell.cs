using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Bullets {
  public class Spell: MonoBehaviour {

    public int damage;
    public float repelInsentive;

    private Rigidbody2D rb2d;

    void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
    }

    public void OnTriggerEnter2D(Collider2D collision) {
      onHit(collision);
    }

    public void OnTriggerStay2D(Collider2D collision) {
      onHit(collision);
    }

    private void onHit(Collider2D collision) {
      if(collision.gameObject.layer == Layers.LayerBullet) {
        return;
      }
      if(collision.gameObject.layer == Layers.LayerCharacter) {
        collision.gameObject.SendMessage(
          MessageNames.DAMAGE_FUNCTION,
          new DamageInfo(damage, rb2d.velocity, repelInsentive),
          SendMessageOptions.DontRequireReceiver);
      }
      Destroy(gameObject);
    }
  }
}

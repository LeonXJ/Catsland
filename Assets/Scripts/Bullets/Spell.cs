using UnityEngine;
using Catsland.Scripts.Common;
using System.Collections;

namespace Catsland.Scripts.Bullets {
  public class Spell: MonoBehaviour {

    public int damage;
    public float repelInsentive;
    public bool onlyHitPlayer = true;
    public float lifetimeInSecond = 5.0f;

    private Rigidbody2D rb2d;

    void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
    }

    public void fire() {
      StartCoroutine(delayDestroy());
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
      if(onlyHitPlayer && collision.gameObject.layer == Layers.LayerCharacter) {
        return;
      }
      if((onlyHitPlayer && collision.gameObject.CompareTag(Tags.PLAYER))
        || (!onlyHitPlayer && collision.gameObject.layer == Layers.LayerCharacter)) {
        collision.gameObject.SendMessage(
          MessageNames.DAMAGE_FUNCTION,
          new DamageInfo(damage, rb2d.velocity, repelInsentive),
          SendMessageOptions.DontRequireReceiver);
      }
      Destroy(gameObject);
    }

    private IEnumerator delayDestroy() {
      yield return new WaitForSeconds(lifetimeInSecond);
      if(gameObject != null) {
        Destroy(gameObject);
      }
    }
  }
}

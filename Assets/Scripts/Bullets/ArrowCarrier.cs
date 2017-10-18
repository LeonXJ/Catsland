using System.Collections;
using UnityEngine;

namespace Catsland.Scripts.Bullets {

  [RequireComponent(typeof(Rigidbody2D))]
  public class ArrowCarrier :MonoBehaviour {


    public int damage = 1;
    public float repelIntensive = 1.0f;
    public string tagForAttachable = "";
    private bool isAttached = false;
    private string tagForOwner;
    private bool isDestroyed = false;

    // References
    private Rigidbody2D rb2d;

    public void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
    }

    public IEnumerator fire(Vector2 direction, float lifetime, string tagForOwner = "") {
      this.tagForOwner = tagForOwner;

      // velocity and orientation
      rb2d.velocity = direction;
      transform.localScale = new Vector2(
        direction.x > 0.0f
            ? Mathf.Abs(transform.localScale.x)
            : -Mathf.Abs(transform.localScale.x),
        1.0f);

      // self destory
      yield return new WaitForSeconds(lifetime);
      if(!isAttached) {
        safeDestroy();
      }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
      if(!isAttached) {
        if(collision.gameObject.CompareTag(tagForAttachable)) {
          isAttached = true;
          rb2d.isKinematic = true;
          // attach to the object
          gameObject.transform.parent = collision.gameObject.transform;
        } else {
          if(!collision.gameObject.CompareTag(tagForOwner)) {
            collision.gameObject.SendMessage(
              BulletConstants.DAMAGE_FUNCTION,
              new DamageInfo(damage, rb2d.velocity, repelIntensive));
            safeDestroy();
          }
        }
      }
    }

    private void safeDestroy() {
      if(!isDestroyed) {
        isDestroyed = true;
        Destroy(gameObject);
      }
    }
  }
}

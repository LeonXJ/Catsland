using UnityEngine;
using Catsland.Scripts.Common;
using System.Collections;

namespace Catsland.Scripts.Bullets {
  public class Spell: MonoBehaviour {

    public int damage;
    public float repelInsentive;
    public bool onlyHitPlayer = true;
    public float lifetimeInSecond = 5.0f;
    public bool isEnable = true;
    public bool destroyWhenHitAny = true;
    public GameObject alsoDestroyGo;
    private float repelDirectionX;
    private bool useRigidbodyRepelDirection = true;


    // Bullet won't collide with the owner. At bullet initial phase, bullet collider might collide
    // with owner's. This bit is useful to avoid bullet hit the owner.
    private GameObject owner;
    private Rigidbody2D rb2d;

    void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
    }

    public void fire(GameObject owner) {
      this.owner = owner;
      StartCoroutine(delayDestroy());
    }

    public void fireWithSpecificRepel(GameObject owner, float repelDirectionX) {
      fire(owner);
      this.repelDirectionX = repelDirectionX;
      this.useRigidbodyRepelDirection = false;
    }

    public void disableCollide() {
      isEnable = false;
    }

    public void OnTriggerEnter2D(Collider2D collision) {
      onHit(collision);
    }

    public void OnTriggerStay2D(Collider2D collision) {
      onHit(collision);
    }

    private void onHit(Collider2D collision) {
      if(!isEnable) {
        return;
      }
      // Ignore other trigger.
      if(collision.isTrigger) {
        return;
      }

      // Bullets won't collide.
      if(collision.gameObject.layer == Layers.LayerBullet) {
        return;
      }
      // Bullets won't collide with owner. This is useful esp. at beginning phase when the bullet
      // collider might collide with owner's.
      if(collision.gameObject == owner) {
        return;
      }

      if((onlyHitPlayer && collision.gameObject.CompareTag(Tags.PLAYER))
        || (!onlyHitPlayer && collision.gameObject.layer == Layers.LayerCharacter)) {
        float repelX = useRigidbodyRepelDirection ? rb2d.velocity.x : repelDirectionX;
        collision.gameObject.SendMessage(
          MessageNames.DAMAGE_FUNCTION,
          new DamageInfo(damage, collision.bounds.center, new Vector2(Mathf.Sign(repelX), 0.0f), repelInsentive),
          SendMessageOptions.DontRequireReceiver);
        HideAndDestroy();
      }
      if(destroyWhenHitAny) {
        HideAndDestroy();
      }
    }

    private IEnumerator delayDestroy() {
      yield return new WaitForSeconds(lifetimeInSecond);
      if(alsoDestroyGo != null) {
        HideAndDestroy();
      } else if(gameObject != null) {
        HideAndDestroy();
      }
    }

    // If the GameObject has trail renderer, hide the sprite and disable the contact
    // function, wait for trail to disappear, then destroy the GameObject.
    private void HideAndDestroy() {
      TrailRenderer trailRenderer = GetComponent<TrailRenderer>();
      if (trailRenderer != null) {
        GetComponent<SpriteRenderer>().enabled = false;
        rb2d.velocity = Vector2.zero;
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        isEnable = false;
        // Hide, until the trail disappear then destroy.
        Destroy(gameObject, trailRenderer.time);
        return;
      }
      Destroy(gameObject);
    }
  }
}

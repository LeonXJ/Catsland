using System.Collections;
using UnityEngine;
using Catsland.Scripts.Common;
using Catsland.Scripts.Misc;

namespace Catsland.Scripts.Bullets {
  [RequireComponent(typeof(Rigidbody2D))]
  public class ArrowCarrier: MonoBehaviour {

    enum ArrowStatus {
      Flying = 0,
      Attached = 1,
      Broken = 2,
      Hit = 3,
    }

    public int damageValue = 1;
    public float repelIntensive = 1.0f;
    public string tagForAttachable = "";
    public bool isAttached = false;
    public float brokenPartSpinSpeed = 4800.0f;
    public float brokenPartBounceSpeedRatio = 0.2f;

    public GameObject brokenArrowPrefab;

    private ArrowStatus status = ArrowStatus.Flying;
    private string tagForOwner;
    private Vector2 velocity;

    private static Random random = new Random();

    // References
    public ParticleSystem particleSystem;
    private SpriteRenderer spriteRenderer;
    private Collider2D collider2d;
    private Rigidbody2D rb2d;
    private Trail trail;

    public void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
      trail = GetComponent<Trail>();
      spriteRenderer = GetComponent<SpriteRenderer>();
      collider2d = GetComponent<Collider2D>();
    }

    public void Update() {
      if(!isAttached && status == ArrowStatus.Flying) {
        rb2d.velocity = new Vector2(velocity.x, rb2d.velocity.y);
      }
    }

    public void fire(Vector2 direction, float lifetime, string tagForOwner) {
      this.tagForOwner = tagForOwner;

      // velocity and orientation
      rb2d.velocity = direction;
      velocity = direction;
      transform.localScale = new Vector2(
        direction.x > 0.0f
            ? Mathf.Abs(transform.localScale.x)
            : -Mathf.Abs(transform.localScale.x),
        1.0f);

      StartCoroutine(expireAndDestroy(lifetime));
    }

    private IEnumerator expireAndDestroy(float lifetime) {
      yield return new WaitForSeconds(lifetime);
      safeDestroy();
    }


    public void OnTriggerEnter2D(Collider2D collision) {
      onArrowHit(collision);
    }

    public void OnTriggerStay2D(Collider2D collision) {
      onArrowHit(collision);
    }

    private void onArrowHit(Collider2D collision) {
      if(isAttached || status != ArrowStatus.Flying) {
        return;
      }

      // Still flying
      // ignore other bullet
      if(collision.gameObject.layer == Layers.LayerBullet) {
        return;
      }
      if(collision.gameObject.layer == Layers.LayerGround) {
        // TODO: support attachable
        // arrow proof by default
        breakArrow();
        return;
      }
      if(collision.gameObject.layer == Layers.LayerCharacter) {
        // not self hurt
        if(tagForOwner != null && collision.gameObject.CompareTag(tagForOwner)) {
          return;
        }
        // TODO: support arrow proof
        // by default vulnerable
        arrowHit(collision);
      }
    }

    public void damage(DamageInfo damageInfo) {
      breakArrow();
      //StartCoroutine(breakArrow());
    }

    private void safeDestroy() {
      if(gameObject != null) {
        Destroy(gameObject);
      }
    }

    private void arrowHit(Collider2D collision) {
      status = ArrowStatus.Hit;
      // emit
      particleSystem.Emit(20);
      particleSystem.gameObject.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

      // set complete arrow status
      spriteRenderer.enabled = false;
      rb2d.velocity = Vector3.zero;
      collider2d.enabled = false;
      gameObject.transform.parent = collision.gameObject.transform;

      // make damage
      collision.gameObject.SendMessage(
        MessageNames.DAMAGE_FUNCTION,
        new DamageInfo(damageValue, rb2d.velocity, repelIntensive),
        SendMessageOptions.DontRequireReceiver);

      // delay self-destory
      safeDestroy();

    }

    private void breakArrow() {
      status = ArrowStatus.Broken;
      GameObject brokenArrow = Instantiate(brokenArrowPrefab);
      brokenArrow.transform.position = transform.position;
      brokenArrow.transform.localScale = transform.lossyScale;

      // adjust layer
      SpriteRenderer[] renderers = brokenArrow.GetComponentsInChildren<SpriteRenderer>();
      foreach(SpriteRenderer renderer in renderers) {
        renderer.sortingOrder = spriteRenderer.sortingOrder;
      }

      // assign random velocity
      Rigidbody2D[] brokenParts = brokenArrow.GetComponentsInChildren<Rigidbody2D>();
      foreach(Rigidbody2D part in brokenParts) {
        part.angularVelocity = (Random.value - 0.5f) * 2.0f * brokenPartSpinSpeed;
        part.velocity = new Vector2(-rb2d.velocity.x * brokenPartBounceSpeedRatio, 0.0f);
      }
      // set complete arrow status 
      spriteRenderer.enabled = false;
      rb2d.velocity = Vector3.zero;
      collider2d.enabled = false;

      // delay self-destory
      safeDestroy();
    }
  }
}

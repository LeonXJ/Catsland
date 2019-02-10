using System.Collections;
using UnityEngine;
using Catsland.Scripts.Common;
using Catsland.Scripts.Misc;
using Catsland.Scripts.Physics;

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

    public bool isShellBreaking = false;


    public GameObject brokenArrowPrefab;
    public GameObject attachedArrowPrefab;
    public float attachedPositionOffset = 0.2f;

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

      // ArrowResult interceptor
      IArrowDamageInterceptor interceptor = collision.gameObject.GetComponent<IArrowDamageInterceptor>();
      if(interceptor != null) {
        ArrowResult result = interceptor.getArrowResult(this);
        switch(result) {
          case ArrowResult.ATTACHED:
            enterAttach(collision);
            return;
          case ArrowResult.BROKEN:
            breakArrow();
            return;
          case ArrowResult.DISAPPEAR:
            safeDestroy();
            return;
          case ArrowResult.HIT:
            arrowHit(collision);
            return;
          case ArrowResult.HIT_AND_BROKEN:
            breakArrow();
            arrowHit(collision);
            return;
          case ArrowResult.SKIP:
            return;
          case ArrowResult.IGNORE:
            break;
        }
      }

      // Ground: attach / break
      if(collision.gameObject.layer == Layers.LayerGround) {
        if(collision.gameObject.CompareTag(Tags.ATTACHABLE)) {
          enterAttach(collision);
          return;
        }
        breakArrow();
        return;
      }

      // Bullet: ignore
      if(collision.gameObject.layer == Layers.LayerBullet) {
        return;
      }

      // Character: ignore / damage
      if(collision.gameObject.layer == Layers.LayerCharacter
        || collision.gameObject.layer == Layers.LayerVulnerableObject) {
        if(tagForOwner != null && collision.gameObject.CompareTag(tagForOwner)) {
          // ignore owner
          return;
        }
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

      // make damage
      collision.gameObject.SendMessage(
        MessageNames.DAMAGE_FUNCTION,
        new DamageInfo(damageValue, collision.bounds.center, rb2d.velocity, repelIntensive),
        SendMessageOptions.DontRequireReceiver);

      // set complete arrow status
      spriteRenderer.enabled = false;
      rb2d.velocity = Vector3.zero;
      collider2d.enabled = false;
      gameObject.transform.parent = collision.gameObject.transform;


      // delay self-destory
      safeDestroy();

    }

    private void enterAttach(Collider2D collision) {
      // This is necessray because another update cycle can happen before self-destroy.
      status = ArrowStatus.Attached;
      GameObject attached = Instantiate(attachedArrowPrefab);
      attached.transform.position = transform.position
        + new Vector3(attachedPositionOffset * transform.lossyScale.x, 0.0f);
      attached.transform.localScale = transform.lossyScale;
      attached.transform.parent = collision.gameObject.transform;
      transferFlame(attached);

      safeDestroy();
    }

    private void breakArrow() {
      status = ArrowStatus.Broken;

      GameObject debrid = BulletUtils.generateDebrid(
        brokenArrowPrefab,
        transform,
        -rb2d.velocity.x * brokenPartBounceSpeedRatio,
        (Random.value - 0.5f) * 2.0f * brokenPartSpinSpeed);

      transferFlame(debrid);

      // set complete arrow status 
      spriteRenderer.enabled = false;
      rb2d.velocity = Vector3.zero;
      collider2d.enabled = false;

      // delay self-destory
      safeDestroy();
    }

    private void transferFlame(GameObject newGO) {
      Flame flame = GetComponentInChildren<Flame>();
      if(flame != null) {
        flame.transform.parent = newGO.transform;
      }
    }
  }
}

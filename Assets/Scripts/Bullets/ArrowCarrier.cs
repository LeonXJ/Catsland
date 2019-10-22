using System.Collections;
using UnityEngine;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;
using Catsland.Scripts.Misc;
using Catsland.Scripts.Physics;

namespace Catsland.Scripts.Bullets {
  [RequireComponent(typeof(Rigidbody2D))]
  public class ArrowCarrier : MonoBehaviour {

    enum ArrowStatus {
      Flying = 0,
      Attached = 1,
      Broken = 2,
      Hit = 3,
    }

    private static readonly string IS_FULL_CHARGED_PARAMETER = "IsFullCharged";

    public int damageValue = 1;
    public float repelIntensive = 1.0f;
    public string tagForAttachable = "";
    public bool isAttached = false;
    public float brokenPartSpinSpeed = 4800.0f;
    public float brokenPartBounceSpeedRatio = 0.2f;

    public bool isShellBreaking { get; private set; } = false;

    public Transform tailPosition;
    public Transform headPosition;

    public GameObject brokenArrowPrefab;
    public GameObject attachedArrowPrefab;
    public float attachedPositionOffset = 0.2f;

    public GameObject hitEffectPrefab;
    public GameObject trailGameObject;
    public GameObject selfDestoryMark;

    [Header("Effect")]
    public GameObject lightLayer;

    private ArrowStatus status = ArrowStatus.Flying;
    private string tagForOwner;
    private Vector2 velocity;

    private static Random random = new Random();

    // References
    public ParticleSystem hitEffectparticleSystem;
    private SpriteRenderer spriteRenderer;
    private Collider2D collider2d;
    private Rigidbody2D rb2d;
    private Trail trail;

    private Vector3 lastTailPosition;

    public void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
      trail = GetComponent<Trail>();
      spriteRenderer = GetComponent<SpriteRenderer>();
      collider2d = GetComponent<Collider2D>();
    }

    public void Start() {
      lastTailPosition = tailPosition.position;
    }

    public void Update() {
      if (!isAttached && status == ArrowStatus.Flying) {
        rb2d.velocity = new Vector2(velocity.x, rb2d.velocity.y);
      }
      if (status == ArrowStatus.Flying) {
        RaycastHit2D[] hits = Physics2D.LinecastAll(lastTailPosition, headPosition.position);
        foreach (RaycastHit2D hit in hits) {
          bool processed = onArrowHitNew(hit);
          if (processed) {
            break;
          }
        }
        lastTailPosition = tailPosition.position;
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

      // mark
      if (selfDestoryMark != null) {
        selfDestoryMark.transform.parent = null;
        selfDestoryMark.GetComponent<Animator>().SetTrigger("start");
        Destroy(selfDestoryMark, .5f);
      }
      safeDestroy();
    }

    private bool onArrowHitNew(RaycastHit2D hit) {

      // ArrowResult interceptor
      Collider2D collider = hit.collider;
      IDamageInterceptor interceptor = hit.collider.gameObject.GetComponent<IDamageInterceptor>();
      if (interceptor != null) {
        ArrowResult result = interceptor.getArrowResult(this);
        switch (result) {
          case ArrowResult.ATTACHED:
          enterAttach(hit);
          return true;
          case ArrowResult.BROKEN:
          breakArrow();
          return true;
          case ArrowResult.DISAPPEAR:
          safeDestroy();
          return true;
          case ArrowResult.HIT:
          arrowHit(collider);
          return true;
          case ArrowResult.HIT_AND_BROKEN:
          breakArrow();
          arrowHit(collider);
          return true;
          case ArrowResult.SKIP:
          return false;
          case ArrowResult.IGNORE:
          return false;
        }
      }

      return defaultHitBehavior(hit);
    }

    public void SetIsShellBreaking(bool isShellBreaking) {
      this.isShellBreaking = isShellBreaking;
      if (lightLayer != null) {
        lightLayer.SetActive(isShellBreaking);
      }
      if (isShellBreaking) {
        gameObject.layer = Common.Layers.LayerSelfIlluminate;
      }
    }

    private bool defaultHitBehavior(RaycastHit2D hit) {

      // Ground: attach / break
      Collider2D collider = hit.collider;
      if (collider.gameObject.layer == Layers.LayerGround) {
        if (collider.gameObject.CompareTag(Tags.ATTACHABLE)) {
          enterAttach(hit);
          return true;
        }
        breakArrow();
        return true;
      }

      /*
      // Bullet: ignore
      if(collider.gameObject.layer == Layers.LayerBullet) {
        return false;
      }

      // Character: ignore / damage
      if(collider.gameObject.layer == Layers.LayerCharacter
        || collider.gameObject.layer == Layers.LayerVulnerableObject) {
        if(tagForOwner != null && collider.gameObject.CompareTag(tagForOwner)) {
          // ignore owner
          return false;
        }
        arrowHit(collider);
        return true;
      }
      */
      return false;
    }

    public void damage(DamageInfo damageInfo) {
      breakArrow();
      //StartCoroutine(breakArrow());
    }

    private void safeDestroy() {
      endTrail();

      if (gameObject != null) {
        Destroy(gameObject);
      }
    }

    private void arrowHit(Collider2D collision) {
      status = ArrowStatus.Hit;
      // emit
      hitEffectparticleSystem.Emit(20);
      hitEffectparticleSystem.gameObject.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

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

    private void enterAttach(RaycastHit2D hit) {
      // This is necessray because another update cycle can happen before self-destroy.
      status = ArrowStatus.Attached;

      // find the exact hit point
      GameObject attached = Instantiate(attachedArrowPrefab);
      attached.transform.position = hit.point - new Vector2(attachedPositionOffset * transform.lossyScale.x, 0.0f);
      attached.transform.localScale = transform.lossyScale;
      attached.transform.parent = hit.collider.gameObject.transform;

      RelayPoint relay = attached.GetComponent<RelayPoint>();
      relay.HideCircle();

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

      // set layer
      debrid.layer = gameObject.layer;
      foreach (Transform t in debrid.transform) {
        t.gameObject.layer = gameObject.layer;
      }
      transferFlame(debrid);

      // set complete arrow status 
      spriteRenderer.enabled = false;
      rb2d.velocity = Vector3.zero;
      collider2d.enabled = false;

      // spark effect
      ReleaseHitEffectParticle();

      // delay self-destory
      safeDestroy();
    }

    private void endTrail() {
      if (trailGameObject != null) {
        trailGameObject.transform.parent = null;
        Destroy(trailGameObject, 2.0f);
      }
    }

    private void transferFlame(GameObject newGO) {
      Flame flame = GetComponentInChildren<Flame>();
      if (flame != null) {
        flame.transform.parent = newGO.transform;
      }
    }

    private void ReleaseHitEffectParticle() {
      if (hitEffectPrefab != null) {
        GameObject hitEffectGo = Instantiate(hitEffectPrefab);
        hitEffectGo.transform.position = transform.position;
        hitEffectGo.transform.localScale = transform.lossyScale;

        ParticleSystem particleSystem = hitEffectGo.GetComponent<ParticleSystem>();
        particleSystem.Play();
      }
    }
  }
}

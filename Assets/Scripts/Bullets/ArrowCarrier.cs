using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;
using Catsland.Scripts.Misc;
using Catsland.Scripts.Physics;
using Catsland.Scripts.Ui;
using System.Linq;
using Catsland.Scripts.Bullets.Arrow;

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

    public float frezeTimeInSecond = .08f;

    public bool isShellBreaking { get; private set; } = false;

    public Transform tailPosition;
    public Transform headPosition;

    public GameObject brokenArrowPrefab;
    public GameObject attachedArrowPrefab;
    public float attachedPositionOffset = 0.2f;

    public GameObject hitEffectPrefab;
    public GameObject trailGameObject;
    public GameObject selfDestoryMark;
    public GameObject hitAndHurtEffectPrefab;

    public SpriteRenderer[] spriteRendererToStopWhenBreak;
    public ParticleSystem[] particleToStopWhenBreak;

    public float frezeSpeed = .1f;
    public float arrowHitMoveableObjectForce = 200f;

    [Header("Party")]
    public Party.WeaponPartyConfig weaponPartyConfig;

    [Header("Effect")]
    public GameObject lightLayer;
    public Sound.Sound arrowBrokenSound;

    private ArrowStatus status = ArrowStatus.Flying;
    private string tagForOwner;
    private Vector2 velocity;

    // References
    public ParticleSystem hitEffectparticleSystem;
    private SpriteRenderer spriteRenderer;
    private Collider2D collider2d;
    private Rigidbody2D rb2d;
    // Only need for shell-breaking arrow.
    private HashSet<GameObject> hitGameObjects;
    private AudioSource audioSource;

    private Vector3 lastTailPosition;
    private Arrow.Rope rope;

    public void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
      spriteRenderer = GetComponent<SpriteRenderer>();
      collider2d = GetComponent<Collider2D>();
      rope = GetComponentInChildren<Arrow.Rope>();
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

    public void fire(Vector2 direction, float lifetime, string tagForOwner, Party.WeaponPartyConfig weaponPartyConfig = null) {
      this.tagForOwner = tagForOwner;
      this.weaponPartyConfig = weaponPartyConfig;

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

      // self destroy mark is shown if the arrow is still flying.
      if (selfDestoryMark != null && status == ArrowStatus.Flying) {
        selfDestoryMark.transform.parent = null;
        selfDestoryMark.GetComponent<Animator>().SetTrigger("start");
        Destroy(selfDestoryMark, .5f);
      }
      StartCoroutine(safeDestroy());
    }

    private bool onArrowHitNew(RaycastHit2D hit) {

      // ArrowResult interceptor
      Collider2D collider = hit.collider;

      // Party filter for character
      if (collider.gameObject.layer == Layers.LayerCharacter && weaponPartyConfig != null) {
        Party colliderParty = collider.gameObject.GetComponent<Party>();
        if (colliderParty != null && !weaponPartyConfig.shouldHitParty(colliderParty)) {
          return false;
        }
      }

      IDamageInterceptor interceptor = hit.collider.gameObject.GetComponent<IDamageInterceptor>();
      if (interceptor != null) {
        ArrowResult result = interceptor.getArrowResult(this);
        switch (result) {
          case ArrowResult.ATTACHED:
          enterAttach(hit);
          return true;
          case ArrowResult.BROKEN:
          playOnlyBrokenSound();
          breakArrow();
          return true;
          case ArrowResult.DISAPPEAR:
          StartCoroutine(safeDestroy());
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

    private void playOnlyBrokenSound() {
      arrowBrokenSound?.PlayOneShot(transform.position);
    }

    private bool defaultHitBehavior(RaycastHit2D hit) {

      // Ground: attach / break
      Collider2D collider = hit.collider;
      if (collider.gameObject.layer == Layers.LayerGround) {
        if (collider.gameObject.CompareTag(Tags.ATTACHABLE)) {
          enterAttach(hit);
          return true;
        }
        playOnlyBrokenSound();
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

    private IEnumerator safeDestroy(float delay = 0.0f) {
      if (status == ArrowStatus.Flying) {
        status = ArrowStatus.Broken;
      }

      yield return new WaitForSeconds(delay);

      spriteRenderer.enabled = false;
      rb2d.bodyType = RigidbodyType2D.Kinematic;
      rb2d.velocity = Vector3.zero;

      // stop particle and flame
      GetComponent<SmallCombustable>()?.exstinguish();

      if (spriteRendererToStopWhenBreak != null) {
        foreach (SpriteRenderer spriteRenderer in spriteRendererToStopWhenBreak) {
          spriteRenderer.enabled = false;
        }
      }
      if (particleToStopWhenBreak != null) {
        foreach (ParticleSystem particleSystem in particleToStopWhenBreak) {
          particleSystem.Stop(true);
        }
      }

      if (gameObject != null) {
        Destroy(gameObject, 5f);
      }
    }

    private void arrowHit(Collider2D collider) {
      if (hitGameObjects != null && hitGameObjects.Contains(collider.gameObject)) {
        // Skip already hit.
        return;
      }

      // emit
      //hitEffectparticleSystem.Emit(20);
      //hitEffectparticleSystem.gameObject.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

      // make damage
      collider.gameObject.SendMessage(
        MessageNames.DAMAGE_FUNCTION,
        new DamageInfo(damageValue, collider.bounds.center, rb2d.velocity, repelIntensive, isShellBreaking: isShellBreaking)
          .setHitCollider(collider),
        SendMessageOptions.DontRequireReceiver);

      // show health bar
      IHealthBarQuery healthBarQuery = collider.gameObject.GetComponent<IHealthBarQuery>();
      if (healthBarQuery != null) {
        SceneConfig.getSceneConfig().GetOpponentHealthBar().ShowForQuery(healthBarQuery);
      } 

      if (isShellBreaking) {
        if (hitGameObjects == null) {
          hitGameObjects = new HashSet<GameObject>();
        }
        hitGameObjects.Add(collider.gameObject);
        StartCoroutine(slowAndResume());
        return;
      }

      status = ArrowStatus.Hit;
      // set complete arrow status
      //spriteRenderer.enabled = false;
      rb2d.velocity = rb2d.velocity.normalized * frezeSpeed;
      collider2d.enabled = false;

      spriteRenderer.material.SetColor("_AmbientLight", Color.white);
      ParticleSystem particle = ReleaseHitEffectParticle(hitAndHurtEffectPrefab);
      //StartCoroutine(slowAndResumeSpeed(0.04f, particle));

      // freeze and destory
      rb2d.bodyType = RigidbodyType2D.Kinematic;
      StartCoroutine(safeDestroy(frezeTimeInSecond));
    }

    private IEnumerator slowAndResumeSpeed(float delay, ParticleSystem particle) {
      ParticleSystem.MainModule main = particle.main;
      main.simulationSpeed = .1f;
      yield return new WaitForSeconds(delay);
      main.simulationSpeed = 1f;
    }

    private IEnumerator slowAndResume() {
      rb2d.velocity = rb2d.velocity.normalized * frezeSpeed;
      rb2d.bodyType = RigidbodyType2D.Kinematic;
      status = ArrowStatus.Hit;

      yield return new WaitForSeconds(frezeTimeInSecond);

      status = ArrowStatus.Flying;
      rb2d.bodyType = RigidbodyType2D.Dynamic;
      rb2d.velocity = velocity;      
    }

    private void enterAttach(RaycastHit2D hit) {
      // This is necessray because another update cycle can happen before self-destroy.
      status = ArrowStatus.Attached;

      // Apply a force to the hit object.
      Rigidbody2D hitRb2d = hit.collider.gameObject.GetComponent<Rigidbody2D>();
      if (hitRb2d != null) {
        hitRb2d.AddForceAtPosition(arrowHitMoveableObjectForce * transform.right, hit.point);
      }
      
      // find the exact hit point
      GameObject attached = Instantiate(attachedArrowPrefab);
      attached.transform.position = hit.point; // - new Vector2(attachedPositionOffset * transform.lossyScale.x, 0.0f);
      attached.transform.rotation = transform.rotation;
      attached.transform.localScale = transform.lossyScale;
      attached.transform.parent = hit.collider.gameObject.transform;

      RelayPoint relay = attached.GetComponent<RelayPoint>();
      relay?.HideCircle();

      transferFlame(attached);

      if (rope != null) {
        rope.transform.parent = attached.transform;
        rope.setAttachedPhysicsGo(hit.collider.gameObject);
        rope.startEffect();
      }

      StartCoroutine(safeDestroy());
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
      ReleaseHitEffectParticle(hitEffectPrefab);

      // delay self-destory
      StartCoroutine(safeDestroy());
    }


    private void transferFlame(GameObject newGO) {
      Flame flame = GetComponentInChildren<Flame>();
      if (flame != null) {
        flame.transform.parent = newGO.transform;
      }
    }

    private ParticleSystem ReleaseHitEffectParticle(GameObject prefab) {
      GameObject hitEffectGo = Instantiate(prefab);
      
      hitEffectGo.transform.position = new Vector3(headPosition.position.x, headPosition.position.y, hitEffectGo.transform.position.z);
      hitEffectGo.transform.localScale = transform.lossyScale;

      ParticleSystem particleSystem = hitEffectGo.GetComponent<ParticleSystem>();
      particleSystem.Play();

      return particleSystem;
    }
  }
}

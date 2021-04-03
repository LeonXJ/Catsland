using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Misc;
using Catsland.Scripts.Ui;

namespace Catsland.Scripts.Controller.Souless {
  public class SoulessController : MonoBehaviour, IHealthBarQuery {

    // Animation state names.
    private const string ANI_STAND = "Souless_Stand";
    private const string ANI_WALK = "Souless_Walk";
    private const string ANI_LAYDOWN = "Laydown";
    private const string ANI_RISE = "Rise";

    // Animation parameters.
    private const string PAR_IS_WALKING = "IsWalking";
    private const string PAR_WANT_CAST = "WantCast";
    private const string PAR_IS_GROUNDED = "IsGrounded";
    private const string PAR_IS_LAYING = "IsLaying";

    [Header("Basic")]
    public Timing timing;
    public string displayName = "Blade";

    [Header("Walk")]
    public float walkSpeedMultiple = 1f;
    public float walkSpeed = 1f;
    public Rect groundSensor;

    [Header("Cast")]
    public GameObject castGameObject;

    [Header("Laydown")]
    public bool isLaydown = true;

    // Right as the direction
    public Transform castPoint;

    // Angle from right.
    public float castDirection = 45f;

    // Right as 0
    public float castSpeed = 10f;

    [Header("Health")]
    public VulnerableAttribute vulnerableAttribute;
    private Bullets.Utils.DamageHelper damageHelper;
    public int headShotMultiple = 5;

    public DamageBypass bodyDamagePart;
    public DamageBypass headDamagePart;

    [Header("Debris")]
    public DebrideGenerator normalDebrideGenerator;
    public DebrideGenerator headShotDebrideGenerator;

    // Whether the last damage is headshot. Used to determine what debris to generate.
    private bool wasHeadShot = false;

    // References
    private Rigidbody2D rb2d;
    private IInput input;
    private Animator animator;

    // Start is called before the first frame update
    void Start() {
      rb2d = GetComponent<Rigidbody2D>();
      input = GetComponent<IInput>();
      animator = GetComponent<Animator>();

      damageHelper = Bullets.Utils.DamageHelper.DamageHelperBuilder.NewBuilder(
        this, vulnerableAttribute, timing)
        .SetOnDie(EnterDie)
        .Build();
    }

    // Update is called once per frame
    void Update() {
      int wantOrientation = Common.Utils.IntSignWithZero(input.getHorizontal());

      if (CanChangeOrientation() && wantOrientation != 0) {
        ControllerUtils.AdjustOrientation(wantOrientation, gameObject);
      }

      // Attack override run
      bool wantAttack = input.attack();

      bool isWalking = false;
      if (CanDetermineVelocity()) {
        if (CanWalk(wantAttack)) {
          rb2d.velocity = new Vector2(wantOrientation * walkSpeed * walkSpeedMultiple, rb2d.velocity.y);
          isWalking= wantOrientation != 0;
        } else {
          rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
        }
      }

      // Update animiation parameters.
      bool isGrounded = isGroundDetected();
      animator.SetBool(PAR_IS_WALKING, isWalking);
      animator.SetBool(PAR_WANT_CAST, wantAttack && CanAttack());
      animator.SetBool(PAR_IS_LAYING, isLaydown);
    }

    // Arrow damage bypassed from sub-component
    public void OnDamageByPass(DamageBypassInfo damageBypassInfo) {
      int damageMultiplier = 1;
      if (damageBypassInfo.byPasser == headDamagePart.gameObject.name) {
        damageMultiplier = headShotMultiple;
        wasHeadShot = true;
      } else {
        wasHeadShot = false;
      }
      damageHelper.OnDamaged(damageBypassInfo.damageInfo, damageMultiplier);
    }
    public float GetOrientation() {
      return transform.lossyScale.x > 0f ? 1f : -1f;
    }

    public void DoCast() {
      var cast = Instantiate(castGameObject);
      cast.transform.position = castPoint.position;

      float orientation = GetOrientation();
      Vector2 direction = Quaternion.Euler(0f, 0f, orientation > 0 ? castDirection : -castDirection) * new Vector3(orientation, 0f, 0f);

      var castRb2d = cast.GetComponent<Rigidbody2D>();
      castRb2d.velocity = direction * castSpeed;
    }

    private bool CanChangeOrientation() {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return baseState.IsName(ANI_STAND) || baseState.IsName(ANI_WALK);
    }

    // Whether the the velocity is under control. Otherwise be subject to inertia.
    private bool CanDetermineVelocity() {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return isGroundDetected() && !baseState.IsName(ANI_LAYDOWN);
    }

    private bool CanWalk(bool wantAttack) {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return !wantAttack && (baseState.IsName(ANI_WALK) || baseState.IsName(ANI_STAND));
    }
    private bool isGroundDetected() {
      return Common.Utils.isRectOverlap(groundSensor, transform, LayerMask.GetMask(Common.Layers.LAYER_GROUND_NAME));
    }

    private bool CanAttack() {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return isGroundDetected() && !baseState.IsName(ANI_LAYDOWN);
    }

    private void EnterDie(DamageInfo damageInfo) {
      if (wasHeadShot) {
        headShotDebrideGenerator?.GenerateDebrides(damageInfo.repelDirection);
      } else {
        normalDebrideGenerator?.GenerateDebrides(damageInfo.repelDirection);
      }
      Destroy(gameObject);
    }
    private void OnDrawGizmosSelected() {
      Common.Utils.drawRectAsGizmos(groundSensor, isGroundDetected() ? Color.white : Color.yellow, transform);
    }

    public HealthCondition GetHealthCondition() {
      return HealthCondition.CreateHealthCondition(vulnerableAttribute, displayName);
    }
  }
}

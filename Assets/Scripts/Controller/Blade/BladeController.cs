using UnityEngine;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Misc;
using Catsland.Scripts.Ui;

namespace Catsland.Scripts.Controller.Blade {
  public class BladeController : MonoBehaviour, IHealthBarQuery {

    // Animation state names.
    private const string ANI_IDLE = "Blade_Idle";
    private const string ANI_RUN = "Blade_Run";
    private const string ANI_SPIN = "Blade_Spin";
    private const string ANI_ATTACK1 = "Blade_Attack1";
    private const string ANI_ATTACK2 = "Blade_Attack2";

    // Animation parameters.
    private const string PAR_IS_RUNNING = "IsRunning";
    private const string PAR_WANT_ATTACK = "WantAttack";
    private const string PAR_IS_GROUNDED = "IsGrounded";

    [Header("Basic")]
    public Timing timing;
    public string displayName = "Blade";

    [Header("Run")]
    public float runSpeed = 6f;

    [Header("Attack")]
    public float attackSlideSpeed = 6f;
    public float attackSlideDuration = 0.5f;
    private float attackSlideRemain = -1f;

    [Header("Spin")]
    public float spinJumpSpeed = 8f;
    public float spinJumpDirection = 60f;
    public Rect groundSensor;

    // The character cannot control the speed in spin. The spin happens in the following order:
    // 1. want spin
    // 2. start spin, animation status stays in NON-SPIN status
    // 3. animation status changes to SPIN
    // 4. animation status changes to NON-SPIN status.
    //
    // Ideally, the character cannot control speed in 2. & 3. However, because 3. can be skipped
    // if the character is blocked, we cannot relay on animation status to cancel the speed control
    // lock. 
    // Current solution is to lock the speed control for a short duration since 2., and relay on
    // animiation status to lock for 3. If 3. is skipped, the lock can still be cancelled.
    public float startSpinLoseSpeedControlDuration = .5f;
    private float startSpinLoseSpeedControlRemain = -1f;

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
      //debrideGenerator = GetComponent<DebrideGenerator>();

      damageHelper = Bullets.Utils.DamageHelper.DamageHelperBuilder.NewBuilder(
        this, vulnerableAttribute, timing)
        .SetOnDie(EnterDie)
        .Build();
    }

    // Update is called once per frame
    void Update() {

      int wantOrientation = Common.Utils.IntSignWithZero(input.getHorizontal());

      // Spin override other
      bool wantSpin = input.jump();
      if (wantSpin && CanStartSpin()) {
        // Apply velocity
        rb2d.velocity =
          Quaternion.AngleAxis(GetOrientation() * spinJumpDirection, Vector3.forward) *
          new Vector3(GetOrientation() * spinJumpSpeed, 0f, 0f);
        startSpinLoseSpeedControlRemain = startSpinLoseSpeedControlDuration;
      }

      // Attack override run
      bool wantAttack = CanAttack(wantSpin) ? input.attack() : false;

      if (CanChangeOrientation() && wantOrientation != 0) {
        ControllerUtils.AdjustOrientation(wantOrientation, gameObject);
      }

      bool isRunning = false;
      if (CanDetermineVelocity()) {
        if (CanRun(wantAttack, wantSpin)) {
          rb2d.velocity = new Vector2(wantOrientation * runSpeed, rb2d.velocity.y);
          isRunning = wantOrientation != 0;
        } else if (IsAttackSliding()) {
          rb2d.velocity = new Vector2(GetOrientation() * attackSlideSpeed, rb2d.velocity.y);
        } else {
          rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
        }
      }

      // Update spin remain.
      if (startSpinLoseSpeedControlRemain > 0f) {
        startSpinLoseSpeedControlRemain -= Time.deltaTime;
      }

      // Update attack sliding remain
      if (attackSlideRemain > 0f){
        AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
        if (baseState.IsName(ANI_ATTACK1) || baseState.IsName(ANI_ATTACK2)) {
          attackSlideRemain -= Time.deltaTime;
        } else {
          attackSlideRemain = -1f;
        }
      }

      // Update animiation parameters.
      bool isGrounded = isGroundDetected();
      animator.SetBool(PAR_IS_RUNNING, isRunning);
      animator.SetBool(PAR_WANT_ATTACK, wantAttack);
      animator.SetBool(PAR_IS_GROUNDED, isGrounded);

      // Update arrow interceptor
      if (isGrounded) {
        headDamagePart.gameObject.GetComponent<ArrowInterceptV2>().arrowResult = ArrowResult.HIT;
        bodyDamagePart.gameObject.GetComponent<ArrowInterceptV2>().arrowResult = ArrowResult.HIT;
      } else {
        headDamagePart.gameObject.GetComponent<ArrowInterceptV2>().arrowResult = ArrowResult.BROKEN;
        bodyDamagePart.gameObject.GetComponent<ArrowInterceptV2>().arrowResult = ArrowResult.BROKEN;
      }
    }
    private bool CanChangeOrientation() {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return baseState.IsName(ANI_IDLE) || baseState.IsName(ANI_RUN);
    }

    // Whether the the velocity is under control. Otherwise be subject to inertia.
    private bool CanDetermineVelocity() {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return startSpinLoseSpeedControlRemain < Mathf.Epsilon && !baseState.IsName(ANI_SPIN);
    }

    private bool IsAttackSliding() {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return attackSlideRemain > 0f && (baseState.IsName(ANI_ATTACK1) || baseState.IsName(ANI_ATTACK2));
    }

    public void StartAttackSliding() {
      attackSlideRemain = attackSlideDuration;
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

    private void EnterDie(DamageInfo damageInfo) {
      if (wasHeadShot) {
        headShotDebrideGenerator?.GenerateDebrides(damageInfo.repelDirection);
      } else {
        normalDebrideGenerator?.GenerateDebrides(damageInfo.repelDirection);
      }
      Destroy(gameObject);
    }

    private bool CanRun(bool wantAttack, bool wantSpin) {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return !wantAttack && !wantSpin && (baseState.IsName(ANI_IDLE) || baseState.IsName(ANI_RUN));
    }

    private bool CanAttack(bool wantSpin) {
      return !wantSpin;
    }

    private bool isGroundDetected() {
      return Common.Utils.isRectOverlap(groundSensor, transform, LayerMask.GetMask(Common.Layers.LAYER_GROUND_NAME));
    }

    public bool IsAttacking() {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return baseState.IsName(ANI_ATTACK1) || baseState.IsName(ANI_ATTACK2);
    }

    public bool IsSpinning() {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return baseState.IsName(ANI_SPIN);
    }

    private bool CanStartSpin() {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return (baseState.IsName(ANI_IDLE) || baseState.IsName(ANI_RUN)) && isGroundDetected();
    }

    private void OnDrawGizmosSelected() {
      Common.Utils.drawRectAsGizmos(groundSensor, isGroundDetected() ? Color.white : Color.yellow, transform);
    }
    public float GetOrientation() {
      return transform.lossyScale.x > 0f ? 1f : -1f;
    }

    public HealthCondition GetHealthCondition() {
      return HealthCondition.CreateHealthCondition(vulnerableAttribute, displayName);
    }
  }
}

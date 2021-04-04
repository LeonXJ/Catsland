using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Common;
using Catsland.Scripts.Misc;
using Catsland.Scripts.Ui;

namespace Catsland.Scripts.Controller.Lunatics {
  public class LunaticsController : MonoBehaviour, IHealthBarQuery {

    // Animation state names.
    private const string ANI_IDLE = "Lunatics_Stand";
    private const string ANI_RUN = "Lunatics_Run";
    private const string ANI_JUMP = "Lunatics_Jump";

    // Animation parameters.
    private const string PAR_IS_RUNNING = "IsRunning";
    private const string PAR_WANT_JUMP = "WantJump";
    private const string PAR_ALARM = "Alarm";


    [Header("Basic")]
    public Timing timing;
    public string displayName = "Lunatics";
    public Rect groundSensor;

    [Header("Run")]
    public float runSpeed = 6f;

    [Header("Jump")]
    public float jumpSpeed = 12f;
    public float jumpSpeedMultiple = 1f;
    public ParticleSystem bladeTrail;
    public GameObject bladeDamagePrefab;

    [Header("Alarm")]
    public ParticleSystem demonAwakeParticle;

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

      if (CanChangeOrientation() && wantOrientation != 0) {
        ControllerUtils.AdjustOrientation(wantOrientation, gameObject);
      }

      bool isGrounded = isGroundDetected();
      bool wantAlarm = input.interact();
      bool willAlarm = false;
      if (wantAlarm && CanAlarm(isGrounded)) {
        willAlarm = true;
      }

      bool wantJump = input.jump();
      bool willJump = false;
      if (wantJump && CanJump(willAlarm)) {
        willJump = true;
      }

      bool isRunning = false;
      if (CanDetermineVelocity()) {
        if (CanRun(willAlarm, willJump)) {
          rb2d.velocity = new Vector2(wantOrientation * runSpeed, rb2d.velocity.y);
          isRunning = wantOrientation != 0;
        } else if (IsJumpSliding()) {
          rb2d.velocity = new Vector2(GetOrientation() * jumpSpeed * jumpSpeedMultiple, rb2d.velocity.y);
        } else {
          rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
        }
      }

      animator.SetBool(PAR_IS_RUNNING, isRunning);
      animator.SetBool(PAR_WANT_JUMP, willJump);
      if (willAlarm) {
        animator.SetTrigger(PAR_ALARM);
      } else {
        animator.ResetTrigger(PAR_ALARM);
      }
    }

    public void ShowBladeTrail() {
      bladeTrail?.Play();

      var bladeDamage = Instantiate(bladeDamagePrefab);
      bladeDamage.transform.position = bladeTrail.transform.position;
    }

    public void EyeFlash() {
      demonAwakeParticle?.Play(true);
    }

    private bool CanChangeOrientation() {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return baseState.IsName(ANI_RUN);
    }

    private bool CanAlarm(bool isGrounded) {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return baseState.IsName(ANI_IDLE) && isGrounded;
    }

    private bool IsJumpSliding() {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return baseState.IsName(ANI_JUMP) ;
    }

    private bool CanJump(bool willAlarm) {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return !willAlarm && (baseState.IsName(ANI_IDLE) || baseState.IsName(ANI_RUN));
    }

    // Whether the the velocity is under control. Otherwise be subject to inertia.
    private bool CanDetermineVelocity() {
      return true;
    }
    private bool CanRun(bool willAlarm, bool willJump) {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return !willAlarm && !willJump && ((baseState.IsName(ANI_IDLE) || baseState.IsName(ANI_RUN)));
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
    private bool isGroundDetected() {
      return Common.Utils.isRectOverlap(groundSensor, transform, LayerMask.GetMask(Common.Layers.LAYER_GROUND_NAME));
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

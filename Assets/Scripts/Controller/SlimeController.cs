using System.Collections;
using UnityEngine;
using DG.Tweening;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Common;
using Catsland.Scripts.Misc;
using Catsland.Scripts.Ui;

namespace Catsland.Scripts.Controller {

  [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Animator)), RequireComponent(typeof(SlimeInput))]
  public class SlimeController : MonoBehaviour, IHealthBarQuery  {

    public interface SlimeInput {
      float getHorizontal();
      bool wantGenerateShield();
    }

    [Header("Common")]
    public string displayName = "Slime";

    [Header("Movement")]
    public Vector2 jumpForce = new Vector2(1.0f, 1.0f);
    public float jumpInternal = 1f;
    public GameObject dustPrefab;
    public Transform dustPosition;
    public float heightToGenerateDust = 5f;
    public GameObject dieEffectPrefab;

    private float maxInAirHeight = 0f;

    [Header("OnDamage")]
    public float frezeTimeInS = .2f;
    public float knockRepelSpeed = 6f;
    public float hitRepelSpeed = 2f;
    public float StrongHitRepelSpeed = 4f;
    public bool canBeKnockback = true;

    [Header("Component")]
    public TriggerBasedSensor groundSensor;

    public int maxHp = 3;
    private int currentHp;

    public int maxSheildHp = 1;
    public int currentShildHp = 0;

    public Timing timing;

    public float shieldGenerationTimeS = 2f;
    // negative means the generation has not been started.
    private float shieldGenerateTimeRemaining = -1f;

    private float wantJumpHorizon = 0.0f;
    private SlimeInput input;
    private Animator animator;
    private Rigidbody2D rb2d;
    private DiamondGenerator diamondGenerator;

    private AnimatorStateInfo currentState;
    private float lastJumpTime = -10000.0f;
    private SlimeEventSounds slimeEventSounds;
    private Vector2 velocityBeforeKnockback;


    private static readonly string STATUS_IDEAL = "Ideal";

    private static readonly string IS_ON_GROUND = "IsOnGround";
    private static readonly string HAS_SHIELD= "HasShield";
    private static readonly string VSpeed = "VSpeed";
    private static readonly string JUMP = "Jump";

    private bool isLastOnGround = false;

    private void Awake() {
      input = GetComponent<SlimeInput>();
      animator = GetComponent<Animator>();
      rb2d = GetComponent<Rigidbody2D>();
      diamondGenerator = GetComponent<DiamondGenerator>();
      slimeEventSounds = GetComponent<SlimeEventSounds>();
      currentHp = maxHp;
    }

    // Update is called once per frame
    void Update() {
      currentState = animator.GetCurrentAnimatorStateInfo(0);
      if (CanMove()) {
        float wantHorizontal = input.getHorizontal();
        if (Mathf.Abs(wantHorizontal) > 0.1f) {
          // jump
          Jump(wantHorizontal);
        }
      }

      if (isGeneratingShield) {
        shieldGenerateTimeRemaining -= Time.deltaTime;
        if (shieldGenerateTimeRemaining < Mathf.Epsilon) {
          GenerateShield();
        }
      }

      if (input.wantGenerateShield()) {
        DoGenerateShild();
      }

      // Ground-based events.
      bool isOnGround = groundSensor.isStay();
      if (!isOnGround) {
        maxInAirHeight = Mathf.Max(maxInAirHeight, transform.position.y);
      }
      if (isOnGround) {
        // Just touch ground.
        // Generate dust.
        if (!isLastOnGround) {
          slimeEventSounds?.PlayLandSound();
          if ((maxInAirHeight - transform.position.y) > heightToGenerateDust && dustPrefab != null) {
            GameObject dust = Instantiate(dustPrefab);
            dust.transform.position = new Vector3(
              dustPosition.position.x, dustPosition.position.y, AxisZ.SPLASH);
            dust.GetComponent<ParticleSystem>()?.Play();
            Destroy(dust, 5f);
          }
        }
        // Reset max in air
        maxInAirHeight = float.MinValue;
      }
      isLastOnGround = isOnGround;

      animator.SetBool(IS_ON_GROUND, isOnGround);
      animator.SetBool(HAS_SHIELD, hasShield);
      animator.SetFloat(VSpeed, rb2d.velocity.y);
    }

    public bool CanMove() {
      return currentState.IsName(STATUS_IDEAL) && (Time.time - lastJumpTime > jumpInternal) && !isGeneratingShield;
    }

    private void Jump(float horizontalSpeed) {
      wantJumpHorizon = horizontalSpeed;
      animator.SetTrigger(JUMP);
    }

    public void DoJump() {
      rb2d.AddForce(new Vector2(Mathf.Sign(wantJumpHorizon) * jumpForce.x, jumpForce.y));
      animator.ResetTrigger(JUMP);
      lastJumpTime = Time.time;
      slimeEventSounds?.PlayJumpOffSound();
    }

    public void damage(DamageInfo damageInfo) {
      damageInfo.onDamageFeedback?.Invoke(new DamageInfo.DamageFeedback(true));

      currentHp -= damageInfo.damage;
      if (currentHp <= 0) {
        enterDie();
        return;
      }
      slimeEventSounds?.PlayOnDamageSound();
      StartCoroutine(freezeThen(frezeTimeInS, damageInfo));
    }
    public float getOrientation() {
      return transform.lossyScale.x > 0.0f ? 1.0f : -1.0f;
    }

    private void enterDie() {
      if (dieEffectPrefab != null) {
        GameObject dieEffect = Instantiate(dieEffectPrefab);
        dieEffect.transform.position = Vector3Builder.From(transform.position).SetZ(AxisZ.SPLASH).Build();
        dieEffect.GetComponent<ParticleSystem>()?.Play(true);
      }

      slimeEventSounds?.PlayOnDieSound();
      diamondGenerator?.GenerateDiamond();
      Destroy(gameObject);
    }

    private IEnumerator freezeThen(float time, DamageInfo damageInfo) {

      float repelSpeed = hitRepelSpeed;
      if (damageInfo.isKnockback()) {
        repelSpeed = knockRepelSpeed;
      }
      if (damageInfo.isSmashAttack) {
        repelSpeed = StrongHitRepelSpeed;
      }

      velocityBeforeKnockback = rb2d.velocity;
      rb2d.velocity = new Vector2(-Mathf.Sign(getOrientation()) * repelSpeed * 0.5f, rb2d.velocity.y);
      rb2d.bodyType = RigidbodyType2D.Kinematic;
      animator.speed = 0f;

      transform.DOShakePosition(time, .15f, 30, 120);

      yield return new WaitForSeconds(time);

      animator.speed = 1f;
      rb2d.bodyType = RigidbodyType2D.Dynamic;
      if (canBeKnockback) {
        Bullets.Utils.ApplyVelocityRepel(damageInfo.repelDirection.normalized * repelSpeed, rb2d);
      } else {
        rb2d.velocity = velocityBeforeKnockback;
      }
    }

    public HealthCondition GetHealthCondition() {
      return new HealthCondition(maxHp, currentHp, displayName);
    }

    public  bool hasShield => currentShildHp > 0;

    public void DoGenerateShild() {
      if (!canGenerateShild()) {
        return;
      }
      shieldGenerateTimeRemaining = shieldGenerationTimeS;
    }

    private bool isGeneratingShield => shieldGenerateTimeRemaining > Mathf.Epsilon;

    public bool canGenerateShild() => groundSensor.isStay() && !hasShield && !isGeneratingShield;

    private void GenerateShield() {
      if (!hasShield) {
        currentShildHp = maxSheildHp;
      }
    }

    public void onShieldHit() {
      currentShildHp -= 1;
      slimeEventSounds.PlayShieldHitSound();
    }
  }
}

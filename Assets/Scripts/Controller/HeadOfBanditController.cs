using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using Catsland.Scripts.Common;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Misc;
using Catsland.Scripts.Ui;

namespace Catsland.Scripts.Controller {
  public class HeadOfBanditController : MonoBehaviour, Bullets.IDamageInterceptor, IMeleeDamageInterceptor, IHealthBarQuery {

    public interface HeadOfBanditInput {
      float getHorizontal();
      bool charge();
      bool jumpSmash();
      bool spell();
      bool display();

      bool unleash();
    }

    public enum Status {
      IDEAL = 1,

      // Display
      DISPLAY,
      DISPLAY_DONE,

      // Charge
      CHARGE_PREPARE,
      CHARGE_CHARGING,
      CHARGE_REST,

      // JumpSmash
      JUMP_SMASH_PREPARE,
      JUMP_SMASH_JUMPING,
      JUMP_SMASH_SMASHING,
      JUMP_SMASH_REST,

      // Spell
      SPELL_PREAPRE,
      SPELL_SPELLING,
      SPELL_REST,

      // DIZZY
      DIZZY,

      DIE_THROW,
      DIE_STAY,

      // Unleash
      UNLEASH,
    }

    private HashSet<Status> immotalStatuses;

    public Status status = Status.IDEAL;

    public float displayTimeInS = 2.0f;

    public string displayName = "Bandit Head";

    // Walk
    public float walkingSpeed = 2.0f;

    // Charge
    [Header("Charge")]
    public float chargeSpeed = 5.0f;
    public float chargePrepareTime = 0.5f;
    public float chargeChargingTime = 2.0f;
    public float chargeRestTime = 1.0f;
    public float chargingShakeAmp = 0.1f;
    public AnimationCurve chargeSpeedCurve;
    private float startChargeTime;

    // Jump Smash
    [Header("Jump Smash")]
    public PolarVector2 jumpSmashJumpVelocity = PolarVector2.FromVector2(new Vector2(6f, 8f));
    public float jumpSmashPrepareTime = 0.3f;
    public float jumpSmashSmashTime = 0.3f;
    public float jumpSmashRestTime = 0.5f;
    public GameObject jumpSmashEffectPrefab;
    public GameObject jumpSmashDustPrefab;
    public Transform jumpSmashEffectTransform;
    public float rippleSpeed = 5f;
    public GameObject ripplePrefab;
    public float jumpSmashShakeAmp = 1.5f;
    private bool isLastOnGround = false;

    public ParticleSystem unleashParticle;

    // Spell
    public GameObject throwingKnifePrefab;
    public Transform knifeGenerationPoint;
    public float knifeSpeed = 8.0f;
    public float knifeAngularSpeed = 360.0f;
    public float spellPrepareTime = 0.3f;
    public float spellSpellTime = 0.3f;
    public float spellRestTime = 0.2f;
    [Range(0f, 1.0f)]
    public float chanceMultiThrow = 0.2f;
    public float multiSpellMaxAngle = 60f;
    [Range(2, 100)]
    public int multiSpellNumber = 5;
    public Transform spellTransform;

    private float currentChargeStatusRemainingTime;
    private LinearSequence displaySequence;
    private LinearSequence chargeSequence;
    private LinearSequence jumpSmashSequence;
    private LinearSequence spellSequence;
    private LinearSequence dieSequence;

    // Dizzy
    public float freezeTimeInS = .3f;
    public float dizzyTimeInS = .3f;
    public float maxRepelForce = 100f;
    public float repelInitSpeed = 5.0f;

    private float dizzyRemainInS = 0f;

    // Die
    public PolarVector2 throwVelocityOnDie = PolarVector2.FromVector2(new Vector2(3f, 3f));
    public GameObject dieEffectGoPrefab;
    public Transform hurtEffectPoint;
    public BattleTrap battleTrap;

    // Health
    public int maxHealth = 5;
    private int curHealth = 5;

    // number of times got damage. Reset when unleash.
    private int accumutateDamageTime = 0;

    // References
    public GameObject groundSensorGo;
    private ISensor groundSensor;
    private Rigidbody2D rb2d;
    private HeadOfBanditInput input;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private CinemachineImpulseSource cinemachineImpulseSource;
    private BanditEventSounds banditEventSounds;
    private DiamondGenerator diamondGenerator;
    private RippleEffect rippleEffect;

    // Animation
    private static readonly string H_SPEED = "HSpeed";
    private static readonly string V_SPEED = "VSpeed";
    private static readonly string JUMP_SMASH_PHASE = "JumpSmashPhase";
    private static readonly string CHARGE_PHASE = "ChargePhase";
    private static readonly string SPELL_PHASE = "SpellPhase";
    private static readonly string DIE = "Die";
    private static readonly string DISPLAY = "Display";
    private static readonly string IS_DIZZY = "IsDizzy";
    private const string UNLEASH = "Unleash";

    private static readonly Dictionary<Status, int> JUMP_SMASH_STATUS_TO_PHASE =
      new Dictionary<Status, int>();
    private static readonly Dictionary<Status, int> CHARGE_STATUS_TO_PHASE =
      new Dictionary<Status, int>();
    private static readonly Dictionary<Status, int> SPELL_STATUS_TO_PHASE =
      new Dictionary<Status, int>();

    static HeadOfBanditController() {
      JUMP_SMASH_STATUS_TO_PHASE.Add(Status.JUMP_SMASH_PREPARE, 1);
      JUMP_SMASH_STATUS_TO_PHASE.Add(Status.JUMP_SMASH_JUMPING, 2);
      JUMP_SMASH_STATUS_TO_PHASE.Add(Status.JUMP_SMASH_SMASHING, 3);
      JUMP_SMASH_STATUS_TO_PHASE.Add(Status.JUMP_SMASH_REST, 4);

      CHARGE_STATUS_TO_PHASE.Add(Status.CHARGE_PREPARE, 1);
      CHARGE_STATUS_TO_PHASE.Add(Status.CHARGE_CHARGING, 2);
      CHARGE_STATUS_TO_PHASE.Add(Status.CHARGE_REST, 3);

      SPELL_STATUS_TO_PHASE.Add(Status.SPELL_PREAPRE, 1);
      SPELL_STATUS_TO_PHASE.Add(Status.SPELL_SPELLING, 2);
      SPELL_STATUS_TO_PHASE.Add(Status.SPELL_REST, 3);
    }


    void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
      input = GetComponent<HeadOfBanditInput>();
      groundSensor = groundSensorGo.GetComponent<ISensor>();
      animator = GetComponent<Animator>();
      spriteRenderer = GetComponent<SpriteRenderer>();
      cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
      banditEventSounds = GetComponent<BanditEventSounds>();
      diamondGenerator = GetComponent<DiamondGenerator>();
      immotalStatuses = new HashSet<Status>();
      immotalStatuses.Add(Status.CHARGE_CHARGING);
      immotalStatuses.Add(Status.JUMP_SMASH_JUMPING);
      immotalStatuses.Add(Status.JUMP_SMASH_SMASHING);
    }

    private void Start() {

      rippleEffect = FindObjectOfType<RippleEffect>();

      displaySequence = LinearSequence.newBuilder()
        .append(Status.DISPLAY, displayTimeInS)
        .append(Status.DISPLAY_DONE, 0.2f)
        .withEndingStatus(Status.IDEAL)
        .build();

      chargeSequence = LinearSequence.newBuilder()
        .append(Status.CHARGE_PREPARE, chargePrepareTime)
        .append(Status.CHARGE_CHARGING, chargeChargingTime)
        .append(Status.CHARGE_REST, chargeRestTime)
        .withEndingStatus(Status.IDEAL)
        .build();

      jumpSmashSequence = LinearSequence.newBuilder()
        .append(Status.JUMP_SMASH_PREPARE, jumpSmashPrepareTime)
        .append(Status.JUMP_SMASH_JUMPING, jumpSmashReadyToSmash)
        .append(Status.JUMP_SMASH_SMASHING, jumpSmashSmashTime)
        .append(Status.JUMP_SMASH_REST, jumpSmashRestTime)
        .withEndingStatus(Status.IDEAL)
        .build();

      spellSequence = LinearSequence.newBuilder()
        .append(Status.SPELL_PREAPRE, spellPrepareTime)
        .append(Status.SPELL_SPELLING, spellSpellTime)
        .append(Status.SPELL_REST, spellRestTime)
        .withEndingStatus(Status.IDEAL)
        .build();

      dieSequence = LinearSequence.newBuilder()
        .append(Status.DIE_THROW, dieThrownOnGround)
        .append(Status.DIE_STAY, 0.1f)
        .withEndingStatus(Status.DIE_STAY)
        .build();

      curHealth = maxHealth;
    }

    void Update() {
      Status oldStatus = status;
      // autonamous logic
      status = (Status)displaySequence.processIfInInterestedStatus(status);
      status = (Status)chargeSequence.processIfInInterestedStatus(status);
      status = (Status)jumpSmashSequence.processIfInInterestedStatus(status);
      status = (Status)spellSequence.processIfInInterestedStatus(status);
      status = (Status)dieSequence.processIfInInterestedStatus(status);

      if (status == Status.DIZZY) {
        dizzyRemainInS -= Time.deltaTime;
        if (dizzyRemainInS <= Mathf.Epsilon) {
          status = Status.IDEAL;
        }
      }

      // transition logic
      if (canDisplay() && input.display()) {
        status = (Status)displaySequence.start();
      }
      float desiredSpeed = input.getHorizontal();
      if (canCharge() && input.charge()) {
        status = (Status)chargeSequence.start();
      }
      if (canJumpSmash() && input.jumpSmash()) {
        Debug.Log("Start Jump.");
        status = (Status)jumpSmashSequence.start();
      }
      if (canSpell() && input.spell()) {
        status = (Status)spellSequence.start();
      }
      if (canUnleash() && input.unleash()) {
        StartUnleash();
      }
      if (canAdjustOrientation()) {
        ControllerUtils.AdjustOrientation(desiredSpeed, gameObject);
      }

      // apply velocity
      if (status == Status.CHARGE_CHARGING) {
        if (startChargeTime < Mathf.Epsilon) {
          startChargeTime = Time.time;
        }
      } else {
        startChargeTime = -Mathf.Epsilon;
      }
      if (status == Status.CHARGE_CHARGING) {
        cinemachineImpulseSource.m_ImpulseDefinition.m_AmplitudeGain = chargingShakeAmp;
        cinemachineImpulseSource.GenerateImpulse();
        float v = chargeSpeed * chargeSpeedCurve.Evaluate((Time.time - startChargeTime) / chargeChargingTime);
        rb2d.velocity = new Vector2(getOrientation() * v, rb2d.velocity.y);
      } else if (status == Status.JUMP_SMASH_JUMPING) {
        if (oldStatus != status) {
          rb2d.velocity = new Vector2(getOrientation() * jumpSmashJumpVelocity.x, jumpSmashJumpVelocity.y);
        }
      } else if (oldStatus != Status.SPELL_SPELLING && status == Status.SPELL_SPELLING) {
        spell();
      } else if (canWalk()) {
        rb2d.velocity = new Vector2(desiredSpeed * walkingSpeed, rb2d.velocity.y);
      } else if (status == Status.DIE_THROW) {
        // do nothing on die throwing
        // fix on ground on die stay
      } else if (groundSensor.isStay()) {
        rb2d.velocity = new Vector2(0.0f, rb2d.velocity.y);
      }

      // effect
      if (!isLastOnGround && groundSensor.isStay()
        && (status == Status.JUMP_SMASH_JUMPING || status == Status.JUMP_SMASH_SMASHING)) {
        if (jumpSmashEffectPrefab != null) {
          GameObject jumpSmashEffect = Instantiate(jumpSmashEffectPrefab);
          jumpSmashEffect.transform.position = jumpSmashEffectTransform.position;
          Destroy(jumpSmashEffect, 1f);
        }

        banditEventSounds.PlayJumpLand();

        if (jumpSmashDustPrefab != null) {
          GameObject jumpSmashDust = Instantiate(jumpSmashDustPrefab);
          jumpSmashDust.transform.position = jumpSmashEffectTransform.position;
          Destroy(jumpSmashDust, 2f);
        }

        // Ripple
        GameObject ripple = Instantiate(ripplePrefab);
        ripple.transform.position = transform.position;
        Rigidbody2D rippleRb2d = ripple.GetComponent<Rigidbody2D>();
        rippleRb2d.velocity = new Vector2(
          getOrientation() * rippleSpeed, 0f);
      }
      isLastOnGround = groundSensor.isStay();

      if (oldStatus != Status.JUMP_SMASH_SMASHING && status == Status.JUMP_SMASH_SMASHING) {
        cinemachineImpulseSource.m_ImpulseDefinition.m_AmplitudeGain = jumpSmashShakeAmp;
        cinemachineImpulseSource.GenerateImpulse();
      }

      if (oldStatus == Status.DIE_THROW && status == Status.DIE_STAY) {
        if (dieEffectGoPrefab != null) {
          GameObject dieEffectGo = Instantiate(dieEffectGoPrefab);
          dieEffectGo.transform.position = hurtEffectPoint.position;
        }
      }

      // animation
      animator.SetFloat(H_SPEED, Mathf.Abs(rb2d.velocity.x));
      animator.SetFloat(V_SPEED, rb2d.velocity.y);
      animator.SetBool(DISPLAY, status == Status.DISPLAY);
      animator.SetBool(IS_DIZZY, status == Status.DIZZY);
      setAnimiatorPhaseValue(JUMP_SMASH_PHASE, JUMP_SMASH_STATUS_TO_PHASE);
      setAnimiatorPhaseValue(CHARGE_PHASE, CHARGE_STATUS_TO_PHASE);
      setAnimiatorPhaseValue(SPELL_PHASE, SPELL_STATUS_TO_PHASE);
      animator.SetBool(DIE, isDead());

      // update sound
      if (oldStatus != Status.CHARGE_CHARGING && status == Status.CHARGE_CHARGING) {
        banditEventSounds.PlayChargeSound();
      }

    }

    public float getOrientation() {
      return transform.lossyScale.x > 0.0f ? 1.0f : -1.0f;
    }

    bool jumpSmashReadyToSmash() {
      return status == Status.JUMP_SMASH_JUMPING && groundSensor.isStay() && !isLastOnGround;
    }

    bool dieThrownOnGround() {
      return status == Status.DIE_THROW && groundSensor.isStay() && !isLastOnGround;
    }

    public bool canAdjustOrientation() {
      return status == Status.IDEAL;
    }

    public bool canCharge() {
      return status == Status.IDEAL;
    }

    public bool canJumpSmash() {
      return status == Status.IDEAL;
    }

    public bool canSpell() {
      return status == Status.IDEAL;
    }

    public bool canUnleash() {
      return status == Status.IDEAL || status == Status.DIZZY;
    }

    public bool canDisplay() {
      return status == Status.IDEAL;
    }

    private void StartUnleash() {
        status = Status.UNLEASH;
        accumutateDamageTime = 0;
        animator.SetTrigger(UNLEASH);
    }

    public void PlayUnleashParticle() {
      unleashParticle?.Play();
    }

    public void PlayUnleashRipple() {
      rippleEffect?.Emit(UnityEngine.Camera.main.WorldToViewportPoint(transform.position));
    }

    // Called by animator.
    public void selfDestroy() {
      Destroy(gameObject);
    }

    public void damage(DamageInfo damageInfo) {
      if (isDead()) {
        return;
      }
      if (immotalStatuses.Contains(status)) {
        damageInfo.onDamageFeedback?.Invoke(new DamageInfo.DamageFeedback(false));
      }

      damageInfo.onDamageFeedback?.Invoke(new DamageInfo.DamageFeedback(true));
      curHealth -= damageInfo.damage;
      accumutateDamageTime++;
      if (curHealth <= 0) {
        enterDie();
        return;
      }
      status = Status.DIZZY;
      dizzyRemainInS = dizzyTimeInS;
      banditEventSounds.PlayOnDamageSound();
      StartCoroutine(freezeThen(freezeTimeInS, damageInfo));
    }

    private IEnumerator freezeThen(float time, DamageInfo damageInfo) {
      rb2d.velocity = Vector2.zero;
      rb2d.bodyType = RigidbodyType2D.Kinematic;

      transform.DOShakePosition(time, .15f, 30, 120);

      yield return new WaitForSeconds(time);

      rb2d.bodyType = RigidbodyType2D.Dynamic;

      rb2d.velocity = damageInfo.repelDirection.normalized * repelInitSpeed;
    }

    public bool isDead() {
      return curHealth <= 0;
    }

    private void enterDie() {
      ContactDamage[] contactDamages = GetComponentsInChildren<ContactDamage>();
      foreach (ContactDamage contactDamage in contactDamages) {
        contactDamage.enabled = false;
      }

      rb2d.velocity = new Vector2(-getOrientation() * throwVelocityOnDie.x, throwVelocityOnDie.y);
      status = (Status)dieSequence.start();

      diamondGenerator?.GenerateDiamond();

      if (battleTrap != null) {
        battleTrap.SetRise(false);
        battleTrap.SetEnable(false);
      }
      banditEventSounds.PlayOnDieSound();
    }

    private bool canWalk() {
      return status == Status.IDEAL;
    }

    private void multiSpell() {
      Debug.Assert(multiSpellNumber > 1, "Multi spell number should >1");

      float halfMaxAngel = multiSpellMaxAngle * 0.5f;
      float step = multiSpellMaxAngle / (multiSpellNumber - 1);
      Vector2 mainDirection = getOrientation() > 0f ? Vector2.right : Vector2.left;

      for (int i = 0; i < multiSpellNumber; i++) {
        float rotateAngle = -halfMaxAngel + step * i;
        Vector2 direction = mainDirection.Rotate(rotateAngle);
        castOneKnife(direction, rotateAngle);
      }
    }

    private void spell() {

      float random = UnityEngine.Random.Range(0f, 1f);
      if (random < chanceMultiThrow) {
        multiSpell();
      } else {
        castOneKnife(getOrientation() > 0 ? Vector2.right : Vector2.left, 0f);
      }
    }

    private void castOneKnife(Vector2 direction, float rotateAngle) {
      GameObject knife = Instantiate(throwingKnifePrefab);
      knife.transform.position = spellTransform.position;
      knife.transform.rotation = Quaternion.AngleAxis(rotateAngle, Vector3.forward);
      knife.transform.localScale = new Vector3((direction.x > 0f ? 1f : -1f), 1f, 1f);

      // velocity
      Rigidbody2D knifeRb2d = knife.GetComponent<Rigidbody2D>();
      knifeRb2d.velocity = direction * knifeSpeed;
      knife.GetComponent<Spell>().fire(gameObject);
    }

    private void setAnimiatorPhaseValue(String variableName, Dictionary<Status, int> statusToPhase) {
      int phase = 0;
      statusToPhase.TryGetValue(status, out phase);
      animator.SetInteger(variableName, phase);
    }

    // Triggerred by animation.
    public void UnleashDone() {
      status = Status.IDEAL;
      animator.ResetTrigger(UNLEASH);
    }

    public int GetAccumulateDamageTime() {
      return accumutateDamageTime;
    }

    public ArrowResult getArrowResult(ArrowCarrier arrowCarrier) {
      if (status == Status.CHARGE_CHARGING
        || status == Status.JUMP_SMASH_JUMPING
        || status == Status.JUMP_SMASH_SMASHING
        || status == Status.UNLEASH) {
        return ArrowResult.BROKEN;
      }
      return ArrowResult.HIT;
    }

    public MeleeResult getMeleeResult() {
      if (status == Status.CHARGE_CHARGING
        || status == Status.JUMP_SMASH_JUMPING
        || status == Status.JUMP_SMASH_SMASHING
        || status == Status.UNLEASH) {
        return MeleeResult.VOID;
      }
      return MeleeResult.HIT;
    }

    public HealthCondition GetHealthCondition() {
      return new HealthCondition(maxHealth, curHealth, displayName);
    }
  }
}

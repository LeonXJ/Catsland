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
    }

    public Vector2 jumpForce = new Vector2(1.0f, 1.0f);
    public float jumpInternalInS = 1.0f;

    public TriggerBasedSensor groundSensor;
    public float frezeTimeInS = .2f;
    public string displayName = "Slime";

    public GameObject dustPrefab;
    public Transform dustPosition;
    public float heightToGenerateDust = 5f;
    private float maxInAirHeight = 0f;

    public float knockRepelSpeed = 6f;
    public float hitRepelSpeed = 2f;
    public float StrongHitRepelSpeed = 4f;

    public GameObject dieEffectPrefab;

    public int maxHp = 3;
    private int currentHp;

    public Timing timing;

    private float wantJumpHorizon = 0.0f;
    private SlimeInput input;
    private Animator animator;
    private Rigidbody2D rb2d;
    private DiamondGenerator diamondGenerator;

    private AnimatorStateInfo currentState;
    private float lastJumpTime = -10000.0f;
    private SlimeEventSounds slimeEventSounds;

    private static readonly string STATUS_IDEAL = "Ideal";

    private static readonly string IS_ON_GROUND = "IsOnGround";
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
      animator.SetFloat(VSpeed, rb2d.velocity.y);
    }

    public bool CanMove() {
      return currentState.IsName(STATUS_IDEAL) && (Time.time - lastJumpTime > jumpInternalInS);
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
      if (diamondGenerator != null) {
        diamondGenerator.Generate(2, 1);
      }
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

      rb2d.velocity = new Vector2(-Mathf.Sign(getOrientation()) * repelSpeed * 0.5f, rb2d.velocity.y);
      rb2d.bodyType = RigidbodyType2D.Kinematic;
      animator.speed = 0f;

      transform.DOShakePosition(time, .15f, 30, 120);

      yield return new WaitForSeconds(time);

      animator.speed = 1f;
      rb2d.bodyType = RigidbodyType2D.Dynamic;
      Bullets.Utils.ApplyVelocityRepel(damageInfo.repelDirection.normalized * repelSpeed, rb2d);
    }

    public HealthCondition GetHealthCondition() {
      return new HealthCondition(maxHp, currentHp, displayName);
    }
  }
}

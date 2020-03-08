using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Catsland.Scripts.Common;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Misc;
using Catsland.Scripts.Ui;
using static Catsland.Scripts.Bullets.Utils;

namespace Catsland.Scripts.Controller {
  [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(BeeInput)), RequireComponent(typeof(Animator))]
  public class BeeController : MonoBehaviour, IHealthBarQuery {

    public interface BeeInput {
      float getHorizontal();
      float getVertical();
      bool attack();
    }

    enum Status {
      FLYING = 0,
      PREPARING = 1,
      CHARING = 2,
      DIZZY = 3,
      DIE = 4,
    }

    private HashSet<Status> immotalStatuses;

    public float flyingSpeed = 1.0f;
    public float prepareTimeInSecond = 1.0f;
    public float chargeSpeed = 5.0f;
    public float chargeTimeInSecond = 0.5f;
    public float dizzyTimeInSecond = 0.5f;
    public int maxHealth = 3;
    public int currentHealth = 3;
    public string displayName = "Bee";

    public float chargeCooldownInSecond = 1.5f;
    private float chargeCooldownRemainInSecond = 0f;

    public AudioSource wingAudioSource;
    public Sound.Sound wingSound;
    public Sound.Sound prepareSound;
    public Sound.Sound chargeSound;

    // Swing attributes
    public float swingCycleInS = 1f;
    public float swingAmp = .5f;
    private float swingPhase = 0f;

    // Knockback
    public float knowbackSpeed = 5f;
    public float maxKnowbackSpeed = 10f;
    public float knowbackDrag = 10f;
    public Sound.Sound damageSound;

    // Die
    public GameObject dieEffectPrefab;
    public float dieRepelVelocity = 10f;
    public float dieRepelAngularSpeed = 900f;
    public Sound.Sound dieSound;

    private Status status = Status.FLYING;
    private Rigidbody2D rb2d;
    private BeeInput input;
    private Animator animator;
    private DiamondGenerator diamondGenerator;

    private float currentPrepareTimeInSecond = 0.0f;
    private ConsumableBool hasCharged = new ConsumableBool();
    private AudioSource audioSource;

    private static readonly string IS_PREPARING = "IsPreparing";
    private static readonly string IS_CHARGING = "IsCharging";
    private static readonly string IS_DIZZY = "IsDizzy";
    private static readonly string IS_DIE = "IsDie";

    private void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
      input = GetComponent<BeeInput>();
      animator = GetComponent<Animator>();
      diamondGenerator = GetComponent<DiamondGenerator>();
      audioSource = GetComponent<AudioSource>();
      immotalStatuses = new HashSet<Status>();
      immotalStatuses.Add(Status.CHARING);
      currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update() {

      Vector2 wantDirection = new Vector2(input.getHorizontal(), input.getVertical()).normalized;

      // Orientation
      if (CanAdjustOrientation()) {
        ControllerUtils.AdjustOrientation(wantDirection.x, gameObject);
      }

      // Flying
      if (CanMoveAround()) {
        swingPhase += Time.deltaTime / swingCycleInS;
        rb2d.velocity = wantDirection * flyingSpeed + new Vector2(0f, swingAmp * Mathf.Sin(swingPhase));
      }

      // Charge
      if (status != Status.CHARING && chargeCooldownRemainInSecond > -Mathf.Epsilon) {
        chargeCooldownRemainInSecond -= Time.deltaTime;
      }

      if (CanPrepare()) {
        if (input.attack()) {
          status = Status.PREPARING;
          rb2d.velocity = Vector2.zero;
          currentPrepareTimeInSecond += Time.deltaTime;
          if (currentPrepareTimeInSecond > prepareTimeInSecond) {
            exitPrepare();
            enterCharge();
          }
        } else {
          exitPrepare();
        }
      }

      // Set animation
      animator.SetBool(IS_PREPARING, status == Status.PREPARING);
      animator.SetBool(IS_CHARGING, status == Status.CHARING);
      animator.SetBool(IS_DIZZY, status == Status.DIZZY);
      animator.SetBool(IS_DIE, status == Status.DIE);

      // Sound
      updateWingSound();
    }

    public bool CanMoveAround() {
      return status == Status.FLYING;
    }

    public bool CanAdjustOrientation() {
      return status == Status.FLYING || status == Status.PREPARING;
    }

    public bool CanPrepare() {
      return (status == Status.FLYING || status == Status.PREPARING) && chargeCooldownRemainInSecond < Mathf.Epsilon;
    }

    public bool consumeHasCharged() {
      return hasCharged.getAndReset();
    }

    private void updateWingSound() {
      if (status == Status.FLYING || status == Status.PREPARING) {
        wingSound.PlayIfNotPlaying(wingAudioSource);
      } else {
        wingAudioSource.Stop();
      }
    }

    private void exitPrepare() {
      status = Status.FLYING;
      currentPrepareTimeInSecond = 0.0f;
    }

    private void enterCharge() {
      status = Status.CHARING;
      hasCharged.setTrue();

      Vector3 targetPosition = SceneConfig.getSceneConfig().GetPlayer().transform.position;
      Vector2 delta = targetPosition - transform.position;
      rb2d.velocity = delta.normalized * chargeSpeed;
      ControllerUtils.AdjustOrientation(delta.x, gameObject);

      StartCoroutine(waitAndStopCharge());
    }

    IEnumerator waitAndStopCharge() {
      yield return new WaitForSeconds(chargeTimeInSecond);
      if (status != Status.DIE) {
        chargeCooldownRemainInSecond = chargeCooldownInSecond;
        status = Status.FLYING;
        rb2d.velocity = Vector2.zero;
      }
    }

    public void damage(DamageInfo damageInfo) {
      if (status == Status.DIE) {
        return;
      }
      if (immotalStatuses.Contains(status)) {
        damageInfo.onDamageFeedback?.Invoke(new DamageInfo.DamageFeedback(false));
        return;
      }

      damageInfo.onDamageFeedback?.Invoke(new DamageInfo.DamageFeedback(true));
      currentHealth -= damageInfo.damage;
      damageSound?.Play(audioSource);
      StartCoroutine(freezeThen(.0f, damageInfo));
    }

    public void PlayPrepareSound() {
      prepareSound?.Play(audioSource);
    }

    public void PlayChargeSound() {
      chargeSound?.Play(audioSource);
    }

    private IEnumerator freezeThen(float time, DamageInfo damageInfo) {
      status = Status.DIZZY;
      rb2d.velocity = Vector2.zero;
      // It makes the knowback effect more strong even if time is set to 0f
      transform.DOShakePosition(time, .15f, 30, 120);

      yield return new WaitForSeconds(time);

      if (status == Status.DIE) {
        yield break;
      }
      if (currentHealth <= 0) {
        enterDie(damageInfo);
      } else {
        rb2d.gravityScale = 2f;
        yield return ApplyVelocityRepel(damageInfo, rb2d, dizzyTimeInSecond, knowbackSpeed, maxKnowbackSpeed, knowbackDrag);
        rb2d.gravityScale = 0f;
        currentPrepareTimeInSecond = 0.0f;
        status = Status.FLYING;
      }
    }

    private void enterDie(DamageInfo damageInfo) {
      if (diamondGenerator != null) {
        diamondGenerator.Generate(5, 1);
      }
      if (dieEffectPrefab != null) {
        GameObject dieEffect = Instantiate(dieEffectPrefab);
        dieEffect.transform.position = new Vector3(transform.position.x, transform.position.y, AxisZ.SPLASH);
        dieEffect.GetComponent<ParticleSystem>()?.Play(true);
        Destroy(dieEffect, 3f);
      }
      dieSound?.PlayOneShot(transform.position);
      wingAudioSource.Stop();

      status = Status.DIE;
      rb2d.freezeRotation = false;
      rb2d.velocity = (damageInfo.repelDirection.normalized + Vector2.up).normalized * dieRepelVelocity;
      rb2d.angularVelocity = dieRepelAngularSpeed;
      rb2d.gravityScale = 1f;
      Destroy(gameObject, 3f);
    }

    public HealthCondition GetHealthCondition() {
      return new HealthCondition(maxHealth, currentHealth, displayName);
    }
  }
}

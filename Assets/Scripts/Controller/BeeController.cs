using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Catsland.Scripts.Common;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Misc;
using static Catsland.Scripts.Bullets.Utils;

namespace Catsland.Scripts.Controller {
  [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(BeeInput)), RequireComponent(typeof(Animator))]
  public class BeeController: MonoBehaviour {

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
    }

    public float flyingSpeed = 1.0f;
    public float prepareTimeInSecond = 1.0f;
    public float chargeSpeed = 5.0f;
    public float chargeTimeInSecond = 0.5f;
    public float dizzyTimeInSecond = 0.5f;
    public int health = 3;

    // Swing attributes
    public float swingCycleInS = 1f;
    public float swingAmp = .5f;
    private float swingPhase = 0f;

    // Knockback
    public float knowbackSpeed = 5f;
    public float maxKnowbackSpeed = 10f;
    public float knowbackDrag = 10f;

    private Status status = Status.FLYING;
    private Rigidbody2D rb2d;
    private BeeInput input;
    private Animator animator;
    private DiamondGenerator diamondGenerator;

    private float currentPrepareTimeInSecond = 0.0f;
    private ConsumableBool hasCharged = new ConsumableBool();

    private static readonly string IS_PREPARING = "IsPreparing";
    private static readonly string IS_CHARGING = "IsCharging";
    private static readonly string IS_DIZZY = "IsDizzy";

    private void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
      input = GetComponent<BeeInput>();
      animator = GetComponent<Animator>();
      diamondGenerator = GetComponent<DiamondGenerator>();
    }

    // Update is called once per frame
    void Update() {

      Vector2 wantDirection = new Vector2(input.getHorizontal(), input.getVertical()).normalized;

      // Orientation
      if(CanAdjustOrientation()) {
        ControllerUtils.AdjustOrientation(wantDirection.x, gameObject);
      }

      // Flying
      if(CanMoveAround()) {
        swingPhase += Time.deltaTime / swingCycleInS;
        rb2d.velocity = wantDirection * flyingSpeed + new Vector2(0f, swingAmp * Mathf.Sin(swingPhase));
      }

      if(CanPrepare()) {
        if(input.attack()) {
          status = Status.PREPARING;
          rb2d.velocity = Vector2.zero;
          currentPrepareTimeInSecond += Time.deltaTime;
          if(currentPrepareTimeInSecond > prepareTimeInSecond) {
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
    }

    public bool CanMoveAround() {
      return status == Status.FLYING;
    }

    public bool CanAdjustOrientation() {
      return status == Status.FLYING || status == Status.PREPARING;
    }

    public bool CanPrepare() {
      return status == Status.FLYING || status == Status.PREPARING;
    }

    public bool consumeHasCharged() {
      return hasCharged.getAndReset();
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
      status = Status.FLYING;
      rb2d.velocity = Vector2.zero;
    }

    public void damage(DamageInfo damageInfo) {
      health -= damageInfo.damage;
      StartCoroutine(freezeThen(.0f, damageInfo));
    }

    private IEnumerator freezeThen(float time, DamageInfo damageInfo) {
      rb2d.velocity = Vector2.zero;
      // It makes the knowback effect more strong even if time is set to 0f
      transform.DOShakePosition(time, .15f, 30, 120);

      yield return new WaitForSeconds(time);

      if (health <= 0) {
        enterDie();
      } else {
        status = Status.DIZZY;
        yield return ApplyVelocityRepel(damageInfo, rb2d, dizzyTimeInSecond, knowbackSpeed, maxKnowbackSpeed, knowbackDrag);
        status = Status.FLYING;
      }
    }

    private void enterDie() {
      if (diamondGenerator != null) {
        diamondGenerator.Generate(5, 1);
      }
      Destroy(gameObject);
    }
  }
}

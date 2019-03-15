using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Animator)), RequireComponent(typeof(BoarInput))]
  public class BoarController: MonoBehaviour, IArrowDamageInterceptor {

    public interface BoarInput {
      float getHorizontal();
      bool prepare();
      bool charge();
      bool throwStone();
    }

    public enum Status {
      IDEAL = 0,
      PREPARE = 1,
      CHARGE = 2,
      THROW = 3,
      CLASH = 4,
    }

    private static readonly Dictionary<Status, string> STATUS_MAP;

    public float chargeSpeed = 5.0f;
    public Rect frontSpaceDetector;
    public LayerMask groundLayerMask;
    public GameObject stonePrefab;
    public Transform stoneGenerationPoint;
    public Vector2 stoneThrowAngleRange = new Vector2(30.0f, 70.0f);
    public int stoneNumber = 5;
    public float stoneSpeed = 8.0f;
    public int maxHealth = 3;

    public Status status = Status.IDEAL;
    public Status wantStatus = Status.IDEAL;
    private Rigidbody2D rb2d;
    private BoarInput input;
    private Animator animator;
    private int currentHealth = 3;

    private ConsumableBool hasCharged = new ConsumableBool();
    private ConsumableBool hasThrowStone = new ConsumableBool();

    private static readonly string IS_PREPARING = "IsPreparing";
    private static readonly string IS_CHARGING = "IsCharging";
    private static readonly string THROW = "Throw";

    static BoarController() {
      STATUS_MAP = new Dictionary<Status, string>();
      STATUS_MAP.Add(Status.IDEAL, "Ideal");
      STATUS_MAP.Add(Status.PREPARE, "Prepare");
      STATUS_MAP.Add(Status.CHARGE, "Charge");
      STATUS_MAP.Add(Status.CLASH, "Clash");
      STATUS_MAP.Add(Status.THROW, "Throw");

    }

    private void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
      input = GetComponent<BoarInput>();
      animator = GetComponent<Animator>();
    }


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

      Vector2 wantDirection = new Vector2(input.getHorizontal(), 0.0f).normalized;
      updateStatusFromAnimator();
      wantStatus = Status.IDEAL;

      if(CanAdjustOrientation()) {
        ControllerUtils.AdjustOrientation(wantDirection.x, gameObject);
      }

      if(CanPrepare() && input.prepare()) {
        enterPrepare();
      }
      if(status == Status.PREPARE && !input.prepare()) {
        exitPrepare();
      }

      if(CanCharge() && input.charge()) {
        enterCharge();
      }
      if(status == Status.CHARGE) {
        if(isGroundDetected(frontSpaceDetector)) {
          exitCharge();
        } else {
          rb2d.velocity = new Vector2(Utils.getOrientation(gameObject) * chargeSpeed, 0.0f);
          wantStatus = Status.CHARGE;
        }
      }

      if(CanThrow() && input.throwStone()) {
        animator.SetTrigger(THROW);
      } else {
        animator.ResetTrigger(THROW);
      }

      // Set animator
      animator.SetBool(
        IS_PREPARING,
        wantStatus == Status.PREPARE
        || wantStatus == Status.CHARGE
        || wantStatus == Status.THROW);
      animator.SetBool(IS_CHARGING, wantStatus == Status.CHARGE);
    }

    public bool CanAdjustOrientation() {
      return status == Status.IDEAL;
    }

    public bool CanPrepare() {
      return status == Status.IDEAL;
    }

    public bool CanCharge() {
      return status == Status.PREPARE;
    }

    public bool CanThrow() {
      return status == Status.PREPARE;
    }

    public void DoThrow() {
      hasThrowStone.setTrue();
      float curAngle = stoneThrowAngleRange.x;
      float stepAngle = (stoneThrowAngleRange.y - stoneThrowAngleRange.x) / stoneNumber;

      while(curAngle < stoneThrowAngleRange.y) {
        GameObject stone = Instantiate(stonePrefab);
        stone.transform.position = stoneGenerationPoint.position;

        Rigidbody2D stoneRb2d = stone.GetComponent<Rigidbody2D>();
        stoneRb2d.velocity = new Vector2(
          stoneSpeed * Mathf.Cos(curAngle * Mathf.Deg2Rad) * Utils.getOrientation(gameObject),
          stoneSpeed * Mathf.Sin(curAngle * Mathf.Deg2Rad));

        curAngle += stepAngle;
      }
    }

    public bool consumeHasCharge() {
      return hasCharged.getAndReset();
    }

    public bool consumeHasThrowStone() {
      return hasThrowStone.getAndReset();
    }

    public void damage(DamageInfo damageInfo) {
      currentHealth -= damageInfo.damage;
      if(currentHealth <= 0) {
        enterDie();
      }
    }

    public void clash() {
      rb2d.velocity = Vector2.zero;
    }

    private void enterDie() {
      Destroy(gameObject);
    }

    private void enterPrepare() {
      wantStatus = Status.PREPARE;
    }

    private void exitPrepare() {
      wantStatus = Status.IDEAL;
    }

    private void enterCharge() {
      wantStatus = Status.CHARGE;
    }

    private void exitCharge() {
      hasCharged.setTrue();
      wantStatus = Status.IDEAL;
    }

    private bool isGroundDetected(Rect rect) {
      return Utils.isRectOverlap(rect, transform, groundLayerMask);
    }

    private void OnDrawGizmosSelected() {
      Utils.drawRectAsGizmos(
        frontSpaceDetector,
        isGroundDetected(frontSpaceDetector) ? Color.white : Color.blue,
        transform);
    }

    private void updateStatusFromAnimator() {
      foreach(KeyValuePair<Status, string> entry in STATUS_MAP) {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName(entry.Value)) {
          status = entry.Key;
          return;
        }
      }
    }

    public ArrowResult getArrowResult(ArrowCarrier arrowCarrier) {
      return arrowCarrier.isShellBreaking ? ArrowResult.HIT : ArrowResult.BROKEN;
    }
  }
}

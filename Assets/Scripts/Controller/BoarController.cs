using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Animator))]
  public class BoarController: MonoBehaviour {

    public enum Status {
      IDEAL = 0,
      PREPARE = 1,
      CHARGE = 2,
    }

    public float chargeSpeed = 5.0f;
    public Rect frontSpaceDetector;
    public LayerMask groundLayerMask;

    public Status status = Status.IDEAL;
    private Rigidbody2D rb2d;
    private DeviceInput input;
    private Animator animator;

    private static readonly string IS_PREPARING = "IsPreparing";
    private static readonly string IS_CHARGING = "IsCharging";

    private void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
      input = GetComponent<DeviceInput>();
      animator = GetComponent<Animator>();
    }


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

      Vector2 wantDirection = new Vector2(input.getHorizontal(), input.getVertical()).normalized;

      if(CanAdjustOrientation()) {
        ControllerUtils.AdjustOrientation(wantDirection.x, gameObject);
      }

      if(CanPrepare() && input.meditation()) {
        enterPrepare();
      }
      if(status == Status.PREPARE && !input.meditation()) {
        exitPrepare();
      }

      if(CanCharge() && input.dash()) {
        enterCharge();
      }
      if(status == Status.CHARGE) {
        if(isGroundDetected(frontSpaceDetector)) {
          exitCharge();
        } else {
          rb2d.velocity = new Vector2(Utils.getOrientation(gameObject) * chargeSpeed, 0.0f);
        }
      }


      // Set animator
      animator.SetBool(IS_PREPARING, status == Status.PREPARE);
      animator.SetBool(IS_CHARGING, status == Status.CHARGE);
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

    private void enterPrepare() {
      status = Status.PREPARE;
    }

    private void exitPrepare() {
      status = Status.IDEAL;
    }

    private void enterCharge() {
      status = Status.CHARGE;
    }

    private void exitCharge() {
      status = Status.IDEAL;
      rb2d.velocity = Vector2.zero;
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
  }
}

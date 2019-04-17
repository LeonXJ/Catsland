using UnityEngine;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ai {
  public class ShellCarrierAi: MonoBehaviour, IInput {

    public LayerMask groundLayerMask;
    public Rect frontSpaceDetector;
    public Rect frontGroundDetector;

    private float horizontal;

    bool IInput.attack() {
      return false;
    }

    bool IInput.dash() {
      return false;
    }

    float IInput.getHorizontal() {
      return horizontal;
    }

    float IInput.getVertical() {
      return 0.0f;
    }

    bool IInput.interact() {
      return false;
    }

    bool IInput.jump() {
      return false;
    }

    bool IInput.meditation() {
      return false;
    }

    // Update is called once per frame
    void Update() {
      horizontal = (canMoveForward() ? 1.0f : -1.0f) *
         Mathf.Sign(Common.Utils.getOrientation(gameObject));
    }

    private bool canMoveForward() {
      return !isGroundDetected(frontSpaceDetector) && isGroundDetected(frontGroundDetector);
    }

    private bool isGroundDetected(Rect rect) {
      return Common.Utils.isRectOverlap(rect, transform, groundLayerMask);
    }

    void OnDrawGizmosSelected() {
      Common.Utils.drawRectAsGizmos(frontSpaceDetector, isGroundDetected(frontSpaceDetector) ? Color.white : Color.blue, transform);
      Common.Utils.drawRectAsGizmos(frontGroundDetector, isGroundDetected(frontGroundDetector) ? Color.white : Color.red, transform);
    }

    public void resetStatus() {
      horizontal = 0.0f;
    }
  }
}


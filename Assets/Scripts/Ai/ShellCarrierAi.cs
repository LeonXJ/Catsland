using UnityEngine;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ai {
  public class ShellCarrierAi: MonoBehaviour, IInput {

    public LayerMask groundLayerMask;
    public LayerMask characterLayerMask;
    public Rect frontCharacterDetector;
    public Rect frontSpaceDetector;
    public Rect frontGroundDetector;

    bool IInput.attack() {
      return isCharacteDetected();
    }

    bool IInput.dash() {
      return false;
    }

    float IInput.getHorizontal() {
      return (canMoveForward() ? 1.0f : -1.0f) *
         Mathf.Sign(Common.Utils.getOrientation(gameObject));
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

    bool IInput.jumpHigher() {
      return false;
    }

    bool IInput.meditation() {
      return false;
    }

    private bool canMoveForward() {
      return !isGroundDetected(frontSpaceDetector) && isGroundDetected(frontGroundDetector);
    }

    private bool isGroundDetected(Rect rect) {
      return Common.Utils.isRectOverlap(rect, transform, groundLayerMask);
    }

    private bool isCharacteDetected() {
      return Common.Utils.isRectOverlap(frontCharacterDetector, transform, characterLayerMask);
    }

    void OnDrawGizmosSelected() {
      Common.Utils.drawRectAsGizmos(frontCharacterDetector, isCharacteDetected() ? Color.white : Color.yellow, transform);
      Common.Utils.drawRectAsGizmos(frontSpaceDetector, isGroundDetected(frontSpaceDetector) ? Color.white : Color.blue, transform);
      Common.Utils.drawRectAsGizmos(frontGroundDetector, isGroundDetected(frontGroundDetector) ? Color.white : Color.red, transform);
    }

    public bool timeSlow() {
      return false;
    }
  }
}


using UnityEngine;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ai {

  // TODO: create a common AI.
  public class CaterpillarAi: MonoBehaviour, CaterpillarController.CaterpillarInput {

    public LayerMask groundLayerMask;
    public Rect frontSpaceDetector;
    public Rect frontGroundDetector;

    private float horizontalSpeed = 0.0f;


    float CaterpillarController.CaterpillarInput.getHorizontal() {
      return horizontalSpeed;
    }

    void Update() {
      horizontalSpeed = (canMoveForward() ? 1.0f : -1.0f) *
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
  }
}
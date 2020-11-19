using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller.Crawler {
  public class CrawerAi : MonoBehaviour, CrawlerController.CrawlerInput {

    public Rect frontSpaceDetector;
    public Rect frontGroundDetector;

    public float GetHorizontal() {
      return (CanMoveForward() ? 1.0f : -1.0f) *
      Mathf.Sign(Utils.getOrientation(gameObject));
    }

    private bool CanMoveForward() {
      return !IsGroundDetected(frontSpaceDetector) && IsGroundDetected(frontGroundDetector);
    }

    private bool IsGroundDetected(Rect rect) {
      return Utils.isRectOverlap(
        rect, transform, LayerMask.GetMask(Layers.LAYER_GROUND_NAME, Layers.LAYER_VULNERABLE_OBJECT));
    }
    void OnDrawGizmosSelected() {
      Utils.drawRectAsGizmos(frontSpaceDetector, IsGroundDetected(frontSpaceDetector) ? Color.white : Color.blue, transform);
      Utils.drawRectAsGizmos(frontGroundDetector, IsGroundDetected(frontGroundDetector) ? Color.white : Color.red, transform);
    }
  }
}

using UnityEngine;

namespace Catsland.Scripts.Controller {
  public static class ControllerUtils {

    // Adjust GameObject's scale according to desired speed.
    public static void AdjustOrientation(float desiredSpeed, GameObject gameObject) {
      float parentLossyScale = gameObject.transform.parent != null
          ? gameObject.transform.parent.lossyScale.x : 1.0f;
      if(desiredSpeed * parentLossyScale > 0.0f) {
        gameObject.transform.localScale = new Vector3(
          Mathf.Abs(gameObject.transform.localScale.x),
          gameObject.transform.localScale.y,
          gameObject.transform.localScale.z);
      }
      if(desiredSpeed * parentLossyScale < 0.0f) {
        gameObject.transform.localScale = new Vector3(
          -Mathf.Abs(gameObject.transform.localScale.x),
          gameObject.transform.localScale.y,
          gameObject.transform.localScale.z);
      }
    }

  }
}

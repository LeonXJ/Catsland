using System;
using System.Collections.Generic;
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

    public static T GetStatusFromAnimator<T>(Animator animator, Dictionary<T, string> statusMap, T defaultStatus) {
      foreach(KeyValuePair<T, string> entry in statusMap) {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName(entry.Value)) {
          return entry.Key;
        }
      }
      return defaultStatus;
    }
  }
}

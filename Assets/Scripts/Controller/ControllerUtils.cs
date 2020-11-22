using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Catsland.Scripts.Bullets;

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

    public static void ApplyHorizontalVelocity(Rigidbody2D rb2d, float inputVelocity, float acceleration, float maxSpeed) {
      if (Mathf.Abs(inputVelocity) > Mathf.Epsilon) {
        rb2d.AddForce(new Vector2(acceleration * inputVelocity, 0f));
        rb2d.velocity = new Vector2(Mathf.Clamp(rb2d.velocity.x, -maxSpeed, maxSpeed), rb2d.velocity.y);
      } else {
        rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
      }
    }

    public static IEnumerator freezeThen(
      Transform transform,
      Rigidbody2D rb2d,
      Animator animator,
      float freezeTime,
      Vector2 velocityDuringFreeze,
      Vector2 velocityAfterFreeze) {

      rb2d.velocity = velocityDuringFreeze;
      rb2d.bodyType = RigidbodyType2D.Kinematic;
      animator.speed = 0f;
      transform.DOShakePosition(freezeTime, .15f, 30, 120);
      yield return new WaitForSeconds(freezeTime);

      animator.speed = 1f;
      rb2d.bodyType = RigidbodyType2D.Dynamic;

      Utils.ApplyVelocityRepel(velocityAfterFreeze, rb2d);
    }

    [System.Serializable]
    public class VibrateConfig {
      public float lowFrequency = .2f;
      public float hightFrequency = .3f;
      public float seconds = .2f;
    }

    public static IEnumerator Vibrate(float lowFrequency, float highFrequency, float seconds) {
      Gamepad.current.SetMotorSpeeds(lowFrequency, highFrequency);

      yield return new WaitForSeconds(seconds);

      Gamepad.current.SetMotorSpeeds(0f, 0f);
    }

    public static IEnumerator Vibrate(VibrateConfig vibrateConfig) {
      return Vibrate(vibrateConfig.lowFrequency, vibrateConfig.hightFrequency, vibrateConfig.seconds);
    }



  }
}

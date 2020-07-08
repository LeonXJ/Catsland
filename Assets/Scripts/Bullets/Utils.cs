using System.Collections;
using UnityEngine;

namespace Catsland.Scripts.Bullets {
  public class Utils {

    public const float MAX_KNOCKBACK_SPEED = 16f;
    public const float KNOCKBACK_DRAG = 20f;

    public static void ApplyRepel(DamageInfo damageInfo, Rigidbody2D rb2d, float maxRepelForce = float.MaxValue) {
      rb2d.velocity = Vector2.zero;
      rb2d.AddForce(damageInfo.repelDirection.normalized * Mathf.Min(damageInfo.repelIntense, maxRepelForce));
    }

    public static void ApplyVelocityRepel(Vector2 velocity, Rigidbody2D rb2d) {
      rb2d.velocity = velocity;
    }

    public static IEnumerator ApplyVelocityRepel(
      DamageInfo damageInfo, Rigidbody2D rb2d, float dizzyTimeInS,
      float knockbackCoherence = 1f, float maxKnockbackSpeed = MAX_KNOCKBACK_SPEED,
      float knockbackDrag = KNOCKBACK_DRAG) {

      rb2d.velocity = damageInfo.repelDirection.normalized
        * Mathf.Clamp(damageInfo.repelIntense * knockbackCoherence, 0f, maxKnockbackSpeed);

      rb2d.drag = knockbackDrag;

      yield return new WaitForSeconds(dizzyTimeInS);

      rb2d.velocity = Vector2.zero;
      rb2d.drag = 0f;
    }
  }
}
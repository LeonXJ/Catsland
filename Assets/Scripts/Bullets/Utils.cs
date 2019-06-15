using UnityEngine;

namespace Catsland.Scripts.Bullets {
  public class Utils {

    public static void ApplyRepel(DamageInfo damageInfo, Rigidbody2D rb2d) {
      rb2d.velocity = Vector2.zero;
      rb2d.AddForce(damageInfo.repelDirection * damageInfo.repelIntense);
    }
  }
}
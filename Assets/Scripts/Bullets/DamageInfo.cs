using UnityEngine;

namespace Catsland.Scripts.Bullets {
  public class DamageInfo {

    public readonly int damage;
    public readonly float repelIntense;
    public readonly Vector2 repelDirection;
    public readonly Vector2 damagePosition;

    public DamageInfo(
      int damage, Vector2 damagePosition, Vector2 repelDirection, float repelIntense) {
      this.damage = damage;
      this.repelDirection = repelDirection;
      this.repelIntense = repelIntense;
      this.damagePosition = damagePosition;
    }
  }
}

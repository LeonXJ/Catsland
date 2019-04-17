using UnityEngine;

namespace Catsland.Scripts.Bullets {
  public class DamageInfo {

    public readonly int damage;
    public readonly float repelIntense;
    public readonly Vector2 repelDirection;
    public readonly Vector2 damagePosition;

    public readonly bool isSmashAttack = false;

    public DamageInfo(
      int damage, Vector2 damagePosition, Vector2 repelDirection, float repelIntense,
      bool isSmashAttack = false) {
      this.damage = damage;
      this.repelDirection = repelDirection;
      this.repelIntense = repelIntense;
      this.damagePosition = damagePosition;
      this.isSmashAttack = isSmashAttack;
    }
  }
}

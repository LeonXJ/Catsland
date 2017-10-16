using UnityEngine;

namespace Catsland.Scripts.Bullets {
  public class DamageInfo :MonoBehaviour {

    public readonly int damage;
    public readonly float repelIntense;
    public readonly Vector2 repelDirection;

    public DamageInfo(int damage, Vector2 repelDirection, float repelIntense) {
      this.damage = damage;
      this.repelDirection = repelDirection;
      this.repelIntense = repelIntense;
    }
  }

}

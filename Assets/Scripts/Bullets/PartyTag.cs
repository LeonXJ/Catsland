using UnityEngine;

namespace Catsland.Scripts.Bullets {
  public class PartyTag : MonoBehaviour {
    public string party;

    public static bool ShouldTakeDamage(PartyTag attacker, PartyTag other) {
      if (attacker == null || other == null) {
        return true;
      }
      return attacker.party != other.party;
    }
  }
}

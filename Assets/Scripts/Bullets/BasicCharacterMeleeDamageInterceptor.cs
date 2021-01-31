using UnityEngine;

namespace Catsland.Scripts.Bullets {
  public class BasicCharacterMeleeDamageInterceptor : MonoBehaviour, IMeleeDamageInterceptor {
    public MeleeResult getMeleeResult() {
      return MeleeResult.HIT;
    }
  }
}

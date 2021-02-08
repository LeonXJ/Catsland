using UnityEngine;

namespace Catsland.Scripts.Bullets {
  public class BasicCharacterMeleeDamageInterceptor : MonoBehaviour, IMeleeDamageInterceptor {

    public MeleeResultStatus meleeResultStatus = MeleeResultStatus.HIT;
    public MeleeResult getMeleeResult() {
      return new MeleeResult(meleeResultStatus);
    }
  }
}

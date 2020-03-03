using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Bullets {
  public class BasicCharacterArrowDammageInterceptor : MonoBehaviour, IDamageInterceptor {

    public bool canBeShoot = true;

    ArrowResult IDamageInterceptor.getArrowResult(ArrowCarrier arrowCarrier) {
      return canBeShoot ? ArrowResult.HIT : ArrowResult.SKIP;
    }
  }
}

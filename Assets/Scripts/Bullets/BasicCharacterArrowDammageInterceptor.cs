using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Bullets {
  public class BasicCharacterArrowDammageInterceptor : MonoBehaviour, IDamageInterceptor {

    public ArrowResult normalArrowResult = ArrowResult.HIT;
    public ArrowResult strongArrowResult = ArrowResult.HIT;

    public bool canBeShoot = true;

    ArrowResult IDamageInterceptor.getArrowResult(ArrowCarrier arrowCarrier) {
      return canBeShoot 
        ? (arrowCarrier.isShellBreaking ? strongArrowResult : normalArrowResult)
        : ArrowResult.SKIP;
    }
  }
}

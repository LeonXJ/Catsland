using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Bullets {
  public class ArrowInterceptV2 : MonoBehaviour, IDamageInterceptor {

    public ArrowResult arrowResult = ArrowResult.HIT;

    public ArrowResult getArrowResult(ArrowCarrier arrowCarrier) {
      return arrowResult;
    }
  }
}

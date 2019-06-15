using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Bullets {
  public class BasicCharacterArrowDammageInterceptor : MonoBehaviour, IDamageInterceptor {
    ArrowResult IDamageInterceptor.getArrowResult(ArrowCarrier arrowCarrier) {
      return ArrowResult.HIT;
    }
  }
}

using UnityEngine;
using System.Collections;

namespace Catsland.Scripts.Bullets {
  public enum ArrowResult {
    // Only destry the arrow.
    DISAPPEAR = 0,

    // Arrow breaks.
    BROKEN = 1,

    // Attach to the object.
    ATTACHED = 2,

    // Arrow ignore the object (flying throught).
    IGNORE = 3,

    // Arrow makes damage on the object.
    HIT = 4,

    // Respect the default behavior, same as no interceptor.
    SKIP = 5,

    // Arrow broken but still hit
    HIT_AND_BROKEN = 6,
  }

  public interface IDamageInterceptor {

    ArrowResult getArrowResult(ArrowCarrier arrowCarrier);
  }
}

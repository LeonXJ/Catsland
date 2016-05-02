using UnityEngine;
using System.Collections;

namespace Catsland {
  public class MathHelper {

    public static Vector2 multiple(Vector2 a, Vector2 b) {
      return new Vector2(a.x * b.x, a.y * b.y);
    }

  }
}

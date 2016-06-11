using UnityEngine;
using System.Collections;

namespace Catslandx {
  public class MathHelper {

    public static Vector2 multiple(Vector2 a, Vector2 b) {
      return new Vector2(a.x * b.x, a.y * b.y);
    }

    public static Vector2 getXY(Vector3 vector3) {
      return new Vector2(vector3.x, vector3.y);
    }

  }
}

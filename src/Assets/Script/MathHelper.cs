using UnityEngine;

namespace Catslandx {
  public class MathHelper {

    public static Vector2 multiple(Vector2 a, Vector2 b) {
      return new Vector2(a.x * b.x, a.y * b.y);
    }

    public static Vector2 getXY(Vector3 vector3) {
      return new Vector2(vector3.x, vector3.y);
    }

    public static Vector3 applyOffset(Vector3 vector3, Vector2 vector2) {
      return new Vector3(vector3.x + vector2.x, vector3.y + vector2.y, vector3.z);
    }

  }
}

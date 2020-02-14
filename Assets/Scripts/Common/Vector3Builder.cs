using UnityEngine;

namespace Catsland.Scripts.Common {
public class Vector3Builder{

    private float x;
    private float y;
    private float z;

    private Vector3Builder(float x, float y, float z) {
      this.x = x;
      this.y = y;
      this.z = z;
    }

    public Vector3Builder SetX(float x) {
      this.x = x;
      return this;
    }
    public Vector3Builder SetY(float y) {
      this.y = y;
      return this;
    }
    public Vector3Builder SetZ(float z) {
      this.z = z;
      return this;
    }

    public Vector3 Build() => new Vector3(x, y, z);

    public static Vector3Builder From(Vector3 vector) =>
      new Vector3Builder(vector.x, vector.y, vector.z);

    public static Vector3Builder From(Vector2 vector) =>
      new Vector3Builder(vector.x, vector.y, 0f);
  }
}

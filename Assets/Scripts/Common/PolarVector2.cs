using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Common {

  [System.Serializable]
  public class PolarVector2 {

    public float angleInDegree;
    public float distance;

    public float angleInRedian => angleInDegree * Mathf.Deg2Rad;
    public float x => Mathf.Cos(angleInRedian) * distance;
    public float y => Mathf.Sin(angleInRedian) * distance;

    public Vector2 GetVector2() {
      // TODO: cache Cos & Sin of angleInDegree when it is changed.
      return new Vector2(x, y);
    }


    public static PolarVector2 FromVector2(Vector2 vector2) {
      PolarVector2 polarVector2 = new PolarVector2();
      polarVector2.distance = vector2.magnitude;
      if (polarVector2.distance > Mathf.Epsilon) {
        polarVector2.angleInDegree = Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg;
      }
      return polarVector2;
    }
  }
}

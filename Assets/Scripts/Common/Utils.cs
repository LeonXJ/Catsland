using UnityEngine;
using System.Collections.Generic;

namespace Catsland.Scripts.Common {
  public class Utils {

    public static float SignWithZero(float num) {
      return num * num > Mathf.Epsilon ? Mathf.Sign(num) : 0f;
    }

    public static int IntSignWithZero(float num) {
      return num > Mathf.Epsilon ? 1 : (num < -Mathf.Epsilon ? -1 : 0);
    }

    public static GameObject getAnyFrom(IEnumerable<GameObject> gameObjects) {
      foreach(GameObject gameObject in gameObjects) {
        return gameObject;
      }
      return null;
    }

    public static void setRelativeRenderLayer(
      SpriteRenderer mainRenderer, SpriteRenderer subRenderer, int offset) {
      subRenderer.sortingOrder = mainRenderer.sortingOrder + offset;
    }

    public static Vector3 overrideXy(Vector3 original, Vector3 other) {
      return new Vector3(other.x, other.y, original.z);
    }

    public static Vector2 toVector2(Vector3 vector) {
      return new Vector2(vector.x, vector.y);
    }

    public static float getOrientation(GameObject gameObject) {
      return gameObject.transform.lossyScale.x > 0.0f ? 1.0f : -1.0f;
    }

    public static void drawRectAsGizmos(Rect rect, Color color, Transform transform = null) {
      Vector3 pos = transform == null ? new Vector3(rect.position.x, rect.position.y, 0f) : transform.TransformPoint(rect.position);
      Vector3 widthOffset = new Vector3(rect.width / 2.0f, 0.0f);
      Vector3 heightOffset = new Vector3(0.0f, rect.height / 2.0f);

      Vector3[] points = new Vector3[4] {
        -widthOffset - heightOffset,
        widthOffset - heightOffset,
        widthOffset + heightOffset,
        -widthOffset + heightOffset
      };

      Gizmos.color = color;
      for(int fromIndex = 0; fromIndex < points.Length; fromIndex++) {
        int toIndex = (fromIndex + 1) % points.Length;
        Gizmos.DrawLine(pos + points[fromIndex], pos + points[toIndex]);
      }
    }
    public static void drawRectAsDebug(Rect rect, Color color, Transform transform = null) {
      Vector3 pos = transformToDebugAxisZ(transform == null ? new Vector3(rect.position.x, rect.position.y, 0f) : transform.TransformPoint(rect.position));
      Vector3 widthOffset = new Vector3(rect.width / 2.0f, 0.0f);
      Vector3 heightOffset = new Vector3(0.0f, rect.height / 2.0f);

      Vector3[] points = new Vector3[4] {
        -widthOffset - heightOffset,
        widthOffset - heightOffset,
        widthOffset + heightOffset,
        -widthOffset + heightOffset
      };

      for(int fromIndex = 0; fromIndex < points.Length; fromIndex++) {
        int toIndex = (fromIndex + 1) % points.Length;
        Debug.DrawLine(pos + points[fromIndex], pos + points[toIndex], color);
      }
    }

    private static Vector3 transformToDebugAxisZ(Vector3 v) => new Vector3(v.x, v.y, AxisZ.DEBUG);

    public static void DrawCircleAsGizmos(float radius, Color color, Vector3 center, int segments = 16) {
      float deltaArc = 2 * Mathf.PI / segments;
      Gizmos.color = color;
      for(int i = 0; i < segments; ++i) {
        float startArc = i * deltaArc;
        float endArc = startArc + deltaArc;
        Gizmos.DrawLine(
          center + new Vector3(Mathf.Sin(startArc), Mathf.Cos(startArc), 0.0f) * radius,
          center + new Vector3(Mathf.Sin(endArc), Mathf.Cos(endArc), 0.0f) * radius);
      }
    }

    public static bool isRectOverlap(Rect rect, Transform transform, LayerMask layerMask) {
      return Physics2D.OverlapBox(
        transform.TransformPoint(rect.position),
        rect.size, 0.0f, layerMask) != null;
    }
  }

  public static class Vector2Extension {
    public static Vector2 Rotate(this Vector2 v, float degrees) {
      return Quaternion.Euler(0, 0, degrees) * v;
    }
  }
}


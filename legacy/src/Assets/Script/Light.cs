using UnityEngine;
using System.Collections;

namespace Catslandx {
  public class Light :MonoBehaviour {

    public float radius = 0.3f;
    public LayerMask whatIsLightBlocker;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

      ArrayList lightShapeVertexes = new ArrayList();
      Collider2D[] colliders = Physics2D.OverlapCircleAll(
        transform.position, radius, whatIsLightBlocker);
      foreach (Collider2D collider in colliders) {
        Vector2[] meshVertexes = getMeshVertexFromCollider(collider);
        if (meshVertexes != null) {
          foreach (Vector2 vertex in meshVertexes) {
            Vector2 toVertexDelta =
              vertex - new Vector2(transform.position.x, transform.position.y);
            RaycastHit2D rayHit = Physics2D.Raycast(
              transform.position, toVertexDelta, radius, whatIsLightBlocker);
            if (rayHit) {
              float toVertexDistanceSquare = toVertexDelta.SqrMagnitude();
              float toHitDistanceSquare = rayHit.distance * rayHit.distance;
              if (toHitDistanceSquare < toVertexDistanceSquare) {
                // the ray is blocked
                continue;
              }
            }
            float rawCos = toVertexDelta.y / toVertexDelta.magnitude;
            if (toVertexDelta.x < 0.0f) {
              rawCos = -rawCos - 1.0f; 
            } 
            if (rayHit) {
              lightShapeVertexes.Add(new LightShapeVertex(rawCos, vertex, rayHit.point));
            } else {
              lightShapeVertexes.Add(new LightShapeVertex(rawCos, vertex));
            }
          }
        }
      }

      // calcualte shape



    }

    Vector2[] boxOffsets = new Vector2[4] {
      new Vector2(0.5f, 0.5f), new Vector2(0.5f, -0.5f), new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, 0.5f)};

    public static Vector2 multiple(Vector2 a, Vector2 b) {
      return new Vector2(a.x * b.x, a.y * b.y);
    }

    // get the world position of the collider vertexes
    Vector2[] getMeshVertexFromCollider(Collider2D collider) {
      if (collider.GetType() == typeof(BoxCollider2D)) {
        BoxCollider2D boxCollider = collider as BoxCollider2D;
        Vector2[] vertexex = new Vector2[4];
        for (int i = 0; i < 4; ++i) {
          vertexex[i] = collider.gameObject.transform.TransformPoint(
            boxCollider.offset + multiple(boxOffsets[i], boxCollider.size));
        }
        return vertexex;
      }
      return null;
    }

    public class LightShapeVertex {
      private readonly float angle;
      private readonly Vector2 checkpoint;
      // endpoint is useful only when hasEndpoint is true
      private readonly Vector2 endpoint;
      private readonly bool hasEndPoint;

      public LightShapeVertex(float angle, Vector2 checkpoint, Vector2 endpoint) {
        this.angle = angle;
        this.checkpoint = checkpoint;
        this.endpoint = endpoint;
        this.hasEndPoint = true;
      }

      public LightShapeVertex(float angle, Vector2 checkpoint) {
        this.angle = angle;
        this.checkpoint = checkpoint;
        this.hasEndPoint = false;
      }
    }
  }
}

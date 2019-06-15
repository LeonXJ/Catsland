using UnityEngine;

namespace OSP {
  public class SpriteRenderGrounp : MonoBehaviour {
    private Bounds bounds;
    private Vector3 previousPosition;

    private void Awake() {
      previousPosition = transform.position;
    }

    public Bounds GetBounds() {
      if (bounds == null || transform.position != previousPosition) {
        UpdateBounds();
      }
      return bounds;
    }

    private void UpdateBounds() {
      bounds = new Bounds(transform.position, Vector3.zero);
      foreach (SpriteRenderer render in GetComponentsInChildren<SpriteRenderer>()) {
        bounds.Encapsulate(render.bounds);
      }
    }
  }
}

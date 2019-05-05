using UnityEngine;

namespace OSP {
  public class SpriteRenderGrounp : MonoBehaviour {
    private Bounds bounds;

    private void Awake() {
      bounds = new Bounds();
      foreach (SpriteRenderer render in GetComponentsInChildren<SpriteRenderer>()) {
        bounds.Encapsulate(render.bounds);
      }
    }

    public Bounds GetBounds() {
      return bounds;
    }
  }
}

using UnityEngine;

namespace Catsland.Scripts.Controller.Archer {
  // TODO: Combine with Player's DrawAnimationV2
  public class DrawAnimation : MonoBehaviour {

    public bool InEffect = false;
    public float Intensity = 0f;
    public SpriteRenderer upperSpriteRenderer;
    public Sprite[] drawingSprites;

    // Start is called before the first frame update
    void Start() {

    }

    private void LateUpdate() {
      if (!InEffect) {
        return;
      }

      int index = (int)Mathf.Ceil(Mathf.Clamp(Intensity, 0.0f, 1.0f) * (drawingSprites.Length - 1));
      upperSpriteRenderer.sprite = drawingSprites[index];
    }
  }
}

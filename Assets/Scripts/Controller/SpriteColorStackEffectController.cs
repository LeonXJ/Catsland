using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class SpriteColorStackEffectController : StackEffectController<SpriteColorConfig> {

    public Color backupSpriteColor = Color.white;
    public bool includeChildren = false;
    private SpriteRenderer renderer;

    private void Awake() {
      base.Awake();
      renderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
      if (includeChildren) {
        foreach (SpriteRenderer childRender in GetComponentsInChildren<SpriteRenderer>()) {
          updateRenderer(childRender);
        }
      } else if (renderer != null) {
        updateRenderer(renderer);
      }
    }

    private void updateRenderer(SpriteRenderer spriteRenderer) {
      Color targetColor = topPrioritizedColor != null ? topPrioritizedColor.color : backupSpriteColor;
      float colorChangeSpeed = topPrioritizedColor != null ? topPrioritizedColor.valueChangeSpeed : backupColorChangeSpeed;
      spriteRenderer.material.SetColor("_Color", Color.Lerp(renderer.material.GetColor("_Color"), targetColor, colorChangeSpeed * Time.deltaTime));
    }

  }
}

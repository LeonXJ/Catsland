using UnityEngine;
using UnityEngine.Tilemaps;

namespace Catsland.Scripts.Controller {
  public class SpriteColorStackEffectController : StackEffectController<SpriteColorConfig> {

    public Color backupSpriteColor = Color.white;
    // Only support spriteRenderer
    public bool includeChildren = false;

    private SpriteRenderer spriteRenderer;
    private TilemapRenderer tileRenderer;


    private void Awake() {
      base.Awake();
      spriteRenderer = GetComponent<SpriteRenderer>();
      tileRenderer = GetComponent<TilemapRenderer>();
    }

    private void Update() {
      if (includeChildren) {
        foreach (SpriteRenderer childRender in GetComponentsInChildren<SpriteRenderer>()) {
          updateRenderer(childRender, null);
        }
      } else {
        updateRenderer(spriteRenderer, tileRenderer);
      }
    }

    private void updateRenderer(SpriteRenderer spriteRenderer, TilemapRenderer tileRenderer) {
      Color targetColor = topPrioritizedColor != null ? topPrioritizedColor.color : backupSpriteColor;
      float colorChangeSpeed = topPrioritizedColor != null ? topPrioritizedColor.valueChangeSpeed : backupColorChangeSpeed;
      if (spriteRenderer != null) {
        spriteRenderer.material.SetColor("_Color", Color.Lerp(spriteRenderer.material.GetColor("_Color"), targetColor, colorChangeSpeed * Time.deltaTime));
      }
      if (tileRenderer != null) {
        tileRenderer.material.SetColor("_Color", Color.Lerp(tileRenderer.material.GetColor("_Color"), targetColor, colorChangeSpeed * Time.deltaTime));
      }
    }
  }
}

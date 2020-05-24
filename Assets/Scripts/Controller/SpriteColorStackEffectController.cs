using UnityEngine;
using UnityEngine.Tilemaps;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  [ExecuteInEditMode]
  public class SpriteColorStackEffectController : StackEffectController<SpriteColorConfig> {

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
      Color targetColor = selectedConfig.color;
      float colorChangeSpeed = selectedConfig.valueChangeSpeed;
      if (spriteRenderer != null) {
        spriteRenderer.material.SetColor(
          Materials.MATERIAL_ATTRIBUTE_TINT,
          Color.Lerp(spriteRenderer.material.GetColor(Materials.MATERIAL_ATTRIBUTE_TINT), targetColor, colorChangeSpeed * Time.deltaTime));
      }
      if (tileRenderer != null) {
        tileRenderer.material.SetColor(
          Materials.MATERIAL_ATTRIBUTE_TINT,
          Color.Lerp(tileRenderer.material.GetColor(Materials.MATERIAL_ATTRIBUTE_TINT), targetColor, colorChangeSpeed * Time.deltaTime));
      }
    }
  }
}

using UnityEngine;
using UnityEngine.Tilemaps;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class SpriteColorStackEffectController : StackEffectController<SpriteColorConfig> {

    // Only support spriteRenderer
    public bool includeChildren = false;

    private SpriteRenderer spriteRenderer;
    private TilemapRenderer tileRenderer;


    private void Awake() {
      spriteRenderer = GetComponent<SpriteRenderer>();
      tileRenderer = GetComponent<TilemapRenderer>();
    }

    private void Start() {
      updateRenderers(true);
    }

    private void Update() {
      updateRenderers(false);
    }

    private void updateRenderers(bool isImmediate) {
      if (includeChildren) {
        foreach (SpriteRenderer childRender in GetComponentsInChildren<SpriteRenderer>()) {
          updateRenderer(childRender, null, isImmediate);
        }
      } else {
        updateRenderer(spriteRenderer, tileRenderer, isImmediate);
      }
    }

    private void updateRenderer(SpriteRenderer spriteRenderer, TilemapRenderer tileRenderer, bool isImmediate) {
      Color targetColor = selectedConfig.color;
      float colorChangeSpeed = selectedConfig.valueChangeSpeed;
      if (spriteRenderer != null) {
        Debug.AssertFormat(spriteRenderer.material.HasProperty(Materials.MATERIAL_ATTRIBUTE_TINT),
          "GameObject ({0})'s material ({1}) doesn't have attribute: {2}.",
          spriteRenderer.gameObject.name, spriteRenderer.material.name, Materials.MATERIAL_ATTRIBUTE_TINT);
        if (isImmediate) {
          spriteRenderer.material.SetColor(Materials.MATERIAL_ATTRIBUTE_TINT, targetColor);

        } else {
          spriteRenderer.material.SetColor(
            Materials.MATERIAL_ATTRIBUTE_TINT,
            Color.Lerp(spriteRenderer.material.GetColor(Materials.MATERIAL_ATTRIBUTE_TINT), targetColor, colorChangeSpeed * Time.deltaTime));
        }
      }
      if (tileRenderer != null) {
        if (isImmediate) {
          tileRenderer.material.SetColor(Materials.MATERIAL_ATTRIBUTE_TINT, targetColor);
        } else {
          tileRenderer.material.SetColor(
            Materials.MATERIAL_ATTRIBUTE_TINT,
            Color.Lerp(tileRenderer.material.GetColor(Materials.MATERIAL_ATTRIBUTE_TINT), targetColor, colorChangeSpeed * Time.deltaTime));
        }
      }
    }
  }
}

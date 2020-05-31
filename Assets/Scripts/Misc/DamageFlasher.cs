using System.Collections;
using UnityEngine;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Misc {
  public class DamageFlasher: MonoBehaviour {
    public Color flashColor = Color.white;
    public bool adjustChildren = false;
    public bool childerenCacheSafe = true;
    public float flashSecond = 0.2f;
    public bool enable = true;
    public SpriteRenderer[] additionalSpriteRenderers;

    private IDamageInterceptor damageInterceptor;
    private SpriteRenderer[] childrenRenderers;

    private SpriteRenderer spriteRenderer;

    private void Awake() {
      spriteRenderer = GetComponent<SpriteRenderer>();
      damageInterceptor = GetComponent<IDamageInterceptor>();
      if(childerenCacheSafe) {
        childrenRenderers = GetComponentsInChildren<SpriteRenderer>();
      }
    }

    void damage(DamageInfo damageInfo) {
      if (!enable) {
        return;
      }
      if(damageInterceptor != null && !damageInterceptor.shouldFlashOnDamage(damageInfo)) {
        return;
      }
      StartCoroutine(flash());
    }

    private IEnumerator flash() {
      if(adjustChildren) {
        foreach(SpriteRenderer renderer in getChildrenRenderers()) {
          renderer.material.SetColor(Materials.MATERIAL_ATTRIBUTE_AMBIENT, flashColor);
        }
      }
      foreach(SpriteRenderer renderer in additionalSpriteRenderers) {
        renderer.material.SetColor(Materials.MATERIAL_ATTRIBUTE_AMBIENT, flashColor);
      }
      spriteRenderer.material.SetColor(Materials.MATERIAL_ATTRIBUTE_AMBIENT, flashColor);

      yield return new WaitForSeconds(flashSecond);

      if(adjustChildren) {
        foreach(SpriteRenderer renderer in getChildrenRenderers()) {
          renderer.material.SetColor(Materials.MATERIAL_ATTRIBUTE_AMBIENT, new Color(0.0f, 0.0f, 0.0f, 0.0f));
        }
      }
        foreach(SpriteRenderer renderer in additionalSpriteRenderers) {
          renderer.material.SetColor(Materials.MATERIAL_ATTRIBUTE_AMBIENT, new Color(0.0f, 0.0f, 0.0f, 0.0f));
        }
      spriteRenderer.material.SetColor(Materials.MATERIAL_ATTRIBUTE_AMBIENT, new Color(0.0f, 0.0f, 0.0f, 0.0f));
    }

    private SpriteRenderer[] getChildrenRenderers() {
      return childerenCacheSafe ? childrenRenderers : GetComponentsInChildren<SpriteRenderer>();
    }
  }
}

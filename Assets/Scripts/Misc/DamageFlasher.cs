using System.Collections;
using UnityEngine;
using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.Misc {
  public class DamageFlasher: MonoBehaviour {
    public Color flashColor = Color.white;
    public bool adjustChildren = false;
    public bool childerenCacheSafe = true;
    public float flashSecond = 0.2f;

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
      if(damageInterceptor != null && !damageInterceptor.shouldFlashOnDamage(damageInfo)) {
        return;
      }
      StartCoroutine(flash());
    }

    private IEnumerator flash() {
      if(adjustChildren) {
        foreach(SpriteRenderer renderer in getChildrenRenderers()) {
          renderer.material.SetColor("_AmbientLight", flashColor);
        }
      }
      spriteRenderer.material.SetColor("_AmbientLight", flashColor);

      yield return new WaitForSeconds(flashSecond);

      if(adjustChildren) {
        foreach(SpriteRenderer renderer in getChildrenRenderers()) {
          renderer.material.SetColor("_AmbientLight", new Color(0.0f, 0.0f, 0.0f, 0.0f));
        }
      }
      spriteRenderer.material.SetColor("_AmbientLight", new Color(0.0f, 0.0f, 0.0f, 0.0f));
    }

    private SpriteRenderer[] getChildrenRenderers() {
      return childerenCacheSafe ? childrenRenderers : GetComponentsInChildren<SpriteRenderer>();
    }
  }
}

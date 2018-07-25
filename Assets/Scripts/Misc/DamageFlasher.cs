using System.Collections;
using UnityEngine;
using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.Misc {
  public class DamageFlasher: MonoBehaviour {
    public Color flashColor = Color.white;
    public float flashSecond = 0.2f;

    private SpriteRenderer spriteRenderer;

    private void Awake() {
      spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void damage(DamageInfo damageInfo) {
      StartCoroutine(flash());
    }

    private IEnumerator flash() {
      spriteRenderer.material.SetColor("_AmbientLight", flashColor);
      yield return new WaitForSeconds(flashSecond);
      spriteRenderer.material.SetColor("_AmbientLight", new Color(0.0f, 0.0f, 0.0f, 0.0f));
    }
  }
}

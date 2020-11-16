using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Misc {
  public class FadeoutAndDestory :MonoBehaviour {

    public float fadeOutInSeconds = 1.0f;

    private SpriteRenderer[] spriteRenders;
    private float passedSeconds = 0.0f;

    private void Awake() {
      spriteRenders = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update() {
      passedSeconds += Time.deltaTime;
      if(passedSeconds > fadeOutInSeconds) {
        Destroy(gameObject);
      } else {
        float alpha = 1.0f - (passedSeconds / fadeOutInSeconds);
        foreach(SpriteRenderer renderer in spriteRenders) {
          Color color = renderer.material.GetColor(Materials.MATERIAL_ATTRIBUTE_TINT);
          color.a = alpha;
          renderer.material.SetColor(Materials.MATERIAL_ATTRIBUTE_TINT, color);
        }
      }
    }
  }
}

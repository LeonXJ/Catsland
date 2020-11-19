using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Misc {
  public class FadeoutAndDestory :MonoBehaviour {

    public float fadeOutInSeconds = 1.0f;

    private SpriteRenderer[] spriteRenders;
    private Light2D[] lights;
    private float[] lightIntensities;

    private float passedSeconds = 0.0f;

    private void Awake() {
      spriteRenders = GetComponentsInChildren<SpriteRenderer>();
      lights = GetComponentsInChildren<Light2D>();
      if (lights.Length > 0) {
        lightIntensities = new float[lights.Length];
        for(int i=0; i<lights.Length; i++) {
          lightIntensities[i] = lights[i].intensity;
        }
      }
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
        for(int i=0; i<lights.Length; i++) {
          lights[i].intensity = lightIntensities[i] * alpha;
        }
      }
    }
  }
}

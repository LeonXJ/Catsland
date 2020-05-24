using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Camera {
  [ExecuteInEditMode]
  public class GlobalLightController: StackEffectController<GlobalLightConfig> {

    [Header("Lights")]
    public Light2D globalForegroundLight;
    public Light2D globalBackgroundLight;

    private void Update() {
      if (globalForegroundLight != null) {
        updateLight(globalForegroundLight, selectedConfig.foregroundColor, selectedConfig.foregroundIntensity, selectedConfig.valueChangeSpeed);
      }
      if (globalBackgroundLight != null) {
        updateLight(globalBackgroundLight, selectedConfig.backgroundColor, selectedConfig.backgroundIntensity, selectedConfig.valueChangeSpeed);
      }
    }

    void updateLight(Light2D light, Color color, float intensity, float changeSpeed) {
      light.color = Color.Lerp(light.color, color, changeSpeed * Time.deltaTime);
      light.intensity = Mathf.Lerp(light.intensity, intensity, changeSpeed * Time.deltaTime);
    }
  }
}

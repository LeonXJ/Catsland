using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Camera {
  [ExecuteInEditMode]
  public class GlobalLightController: StackEffectController<GlobalLightConfig> {

    public Color backupAmbientColor = Color.white;
    public Light2D globalLight;

    void Awake() {
      base.Awake();
    }

    private void Update() {
      if (globalLight != null) {
        Color targetColor = topPrioritizedColor != null ? topPrioritizedColor.color : backupAmbientColor;
        float colorChangeSpeed = topPrioritizedColor != null ? topPrioritizedColor.valueChangeSpeed : backupColorChangeSpeed;
        globalLight.color =
          Color.Lerp(globalLight.color, targetColor, colorChangeSpeed * Time.deltaTime);
      
      }
    }
  }
}

using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Camera {
  [ExecuteInEditMode]
  public class GlobalLightController: StackEffectController<GlobalLightConfig> {

    public Color backupAmbientColor = Color.white;
    private UnityEngine.Camera lightmapCamera;

    void Awake() {
      base.Awake();
      lightmapCamera = GetComponent<UnityEngine.Camera>();
    }

    private void Update() {
      if(lightmapCamera != null) {
        Color targetColor = topPrioritizedColor != null ? topPrioritizedColor.color : backupAmbientColor;
        float colorChangeSpeed = topPrioritizedColor != null ? topPrioritizedColor.valueChangeSpeed : backupColorChangeSpeed;
        lightmapCamera.backgroundColor =
          Color.Lerp(lightmapCamera.backgroundColor, targetColor, colorChangeSpeed * Time.deltaTime);
      }
    }
  }
}

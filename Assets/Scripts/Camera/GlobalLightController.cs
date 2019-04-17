using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Camera {
  [ExecuteInEditMode]
  public class GlobalLightController: StackEffectController {

    private UnityEngine.Camera lightmapCamera;

    void Awake() {
      base.Awake();
      lightmapCamera = GetComponent<UnityEngine.Camera>();
    }

    private void Update() {
      if(lightmapCamera != null) {
        Color targetColor = topPrioritizedColor != null ? topPrioritizedColor.color : backupAmbientColor;
        float colorChangeSpeed = topPrioritizedColor != null ? topPrioritizedColor.colorChangeSpeed : backupColorChangeSpeed;
        lightmapCamera.backgroundColor =
          Color.Lerp(lightmapCamera.backgroundColor, targetColor, colorChangeSpeed);
      }
    }
  }
}

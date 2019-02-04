using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Camera {
  [ExecuteInEditMode]
  public class GlobalLightController: MonoBehaviour {

    public Color backupAmbientColor = Color.white;
    public float backupColorChangeSpeed = 0.1f;
    public Dictionary<string, GlobalLightConfig> globalLightColors;

    private UnityEngine.Camera lightmapCamera;
    private GlobalLightConfig topPrioritizedColor;

    void Awake() {
      lightmapCamera = GetComponent<UnityEngine.Camera>();
      globalLightColors = new Dictionary<string, GlobalLightConfig>();
    }

    public bool RegisterColor(GlobalLightConfig color) {
      if(!globalLightColors.ContainsKey(color.name)) {
        globalLightColors.Add(color.name, color);
        updateTopPriority();
        return true;
      }
      return false;
    }

    public bool UnregisterColor(GlobalLightConfig color) {
      if(globalLightColors.ContainsKey(color.name)) {
        globalLightColors.Remove(color.name);
        updateTopPriority();
        return true;
      }
      return false;
    }

    private void updateTopPriority() {
      int topPriority = int.MinValue;
      topPrioritizedColor = null;
      foreach(GlobalLightConfig color in globalLightColors.Values) {
        if(color.priority > topPriority) {
          topPriority = color.priority;
          topPrioritizedColor = color;
        }
      }
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

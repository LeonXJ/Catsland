using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller {
  public abstract class StackEffectController: MonoBehaviour {

    public Color backupAmbientColor = Color.white;
    public float backupColorChangeSpeed = 0.1f;
    public Dictionary<string, StackEffectConfig> stackEffectConfigs;

    protected StackEffectConfig topPrioritizedColor;

    protected virtual void Awake() {
      stackEffectConfigs = new Dictionary<string, StackEffectConfig>();
    }

    public bool RegisterColor(StackEffectConfig color) {
      if(!stackEffectConfigs.ContainsKey(color.name)) {
        stackEffectConfigs.Add(color.name, color);
        updateTopPriority();
        return true;
      }
      return false;
    }

    public bool UnregisterColor(StackEffectConfig color) {
      if(stackEffectConfigs.ContainsKey(color.name)) {
        stackEffectConfigs.Remove(color.name);
        updateTopPriority();
        return true;
      }
      return false;
    }

    private void updateTopPriority() {
      int topPriority = int.MinValue;
      topPrioritizedColor = null;
      foreach(StackEffectConfig color in stackEffectConfigs.Values) {
        if(color.priority > topPriority) {
          topPriority = color.priority;
          topPrioritizedColor = color;
        }
      }
    }
  }
}

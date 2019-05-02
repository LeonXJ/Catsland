using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller {
  public abstract class StackEffectController<T>: MonoBehaviour where T : StackEffectConfig {

    public float backupColorChangeSpeed = 0.1f;
    public Dictionary<string, T> stackEffectConfigs;

    protected T topPrioritizedColor;

    protected virtual void Awake() {
      stackEffectConfigs = new Dictionary<string, T>();
    }

    public bool RegisterColor(T color) {
      if(!stackEffectConfigs.ContainsKey(color.name)) {
        stackEffectConfigs.Add(color.name, color);
        updateTopPriority();
        return true;
      }
      return false;
    }

    public bool UnregisterColor(T color) {
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
      foreach(T color in stackEffectConfigs.Values) {
        if(color.priority > topPriority) {
          topPriority = color.priority;
          topPrioritizedColor = color;
        }
      }
    }
  }
}

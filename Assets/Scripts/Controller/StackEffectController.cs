using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller {
  public abstract class StackEffectController<T>: MonoBehaviour where T : StackEffectConfig {

    public Dictionary<string, T> stackEffectConfigs;

    public T backupConfig;

    [Header("Debug")]
    public T debugDiffConfig;
    public bool debugUseDebugDiffConfig = false;

    protected T selectedConfig => debugUseDebugDiffConfig ? debugDiffConfig : topPrioritizedConfig ?? backupConfig;
    protected T topPrioritizedConfig;

    protected virtual void Awake() {
      stackEffectConfigs = new Dictionary<string, T>();
    }

    public bool RegisterColor(T color) {

      Debug.LogFormat("Register color: {0}.", color.name);

      if(!stackEffectConfigs.ContainsKey(color.name)) {
        stackEffectConfigs.Add(color.name, color);
        updateTopPriority();
        return true;
      }
      return false;
    }

    public bool UnregisterColor(T color) {
      Debug.LogFormat("Unregister color: {0}.", color.name);
      if(stackEffectConfigs.ContainsKey(color.name)) {
        stackEffectConfigs.Remove(color.name);
        updateTopPriority();
        return true;
      }
      return false;
    }

    private void updateTopPriority() {
      int topPriority = int.MinValue;
      topPrioritizedConfig = null;
      foreach(T color in stackEffectConfigs.Values) {
        if(color.priority > topPriority) {
          topPriority = color.priority;
          topPrioritizedConfig = color;
        }
      }
    }
  }
}

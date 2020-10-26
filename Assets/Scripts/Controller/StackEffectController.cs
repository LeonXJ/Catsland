using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller {
  public abstract class StackEffectController<T>: MonoBehaviour where T : StackEffectConfig {

    public Dictionary<string, T> stackEffectConfigs = new Dictionary<string, T>();

    public T backupConfig;
    public bool mute = false;

    [Header("Debug")]
    public T debugDiffConfig;
    public bool debugUseDebugDiffConfig = false;

    protected T selectedConfig => debugUseDebugDiffConfig
      ? debugDiffConfig
      : ((mute || topPrioritizedConfig == null) ? backupConfig : topPrioritizedConfig);
    protected T topPrioritizedConfig;

    private static string getConfigName(T config) {
      return string.IsNullOrEmpty(config.channelName) ? config.name : config.channelName;
    }

    public bool RegisterConfig(T config, bool overwrite = false) {
      string name = getConfigName(config);
      if (stackEffectConfigs.ContainsKey(name)) {
        if (!overwrite) {
          return false;
        }
        // Overwrite, then remove the existing key.
        stackEffectConfigs.Remove(name);
      }

      stackEffectConfigs.Add(name, config);
      updateTopPriority();
      return true;
    }

    public bool UnregisterConfig(T config) {
      return UnregisterConfig(getConfigName(config));
    }

    public bool UnregisterConfig(string name) {
      Debug.LogFormat("Unregister color: {0}.", name);
      if(stackEffectConfigs.ContainsKey(name)) {
        stackEffectConfigs.Remove(name);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Progress {
  public class GameProgress : MonoBehaviour {

    public enum ProgressKey {
      DEFEAT_BANDIT = 1,
    }

    private static GameProgress instance;
    private Dictionary<ProgressKey, string> progress = new Dictionary<ProgressKey, string>();

    public void setProgress(ProgressKey key, string value, bool overwrite = true) {
      if (!progress.ContainsKey(key)) {
        progress.Add(key, value);
        return;
      }
      if (!overwrite) {
        return;
      }
      progress[key] = value;
    }

    public void setProgress(ProgressKey key, bool value, bool overwrite = true) {
      setProgress(key, value.ToString(), overwrite);
    }

    public bool? getBoolProgress(ProgressKey key) {
      string stringValue = getProgress(key);
      if (stringValue != null) {
        bool success = bool.TryParse(stringValue, out bool result);
        if (success) {
          return result;
        }
      }
      return null;
    }

    public string getProgress(ProgressKey key) {
      if (!progress.ContainsKey(key)) {
        return null;
      }
      return progress[key];
    }


    public static GameProgress getInstance() {
      return instance;
    }

    private void Awake() {
      instance = this;
      DontDestroyOnLoad(gameObject);
    }
  }
}

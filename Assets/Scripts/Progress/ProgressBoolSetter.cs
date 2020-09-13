using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Progress {
  public class ProgressBoolSetter : MonoBehaviour {

    public GameProgress.ProgressKey key;
    public bool value;

    public void setValue() {
      Debug.Log("Set Game progress value, " + key + ": " + value);
      GameProgress.getInstance().setProgress(key, value);
    }
  }
}

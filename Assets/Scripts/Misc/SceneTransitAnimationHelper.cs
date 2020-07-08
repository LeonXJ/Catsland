using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Misc {
  public class SceneTransitAnimationHelper : MonoBehaviour {

    private SceneMaster sceneMaster;

    private void Awake() {
      sceneMaster = FindObjectOfType<SceneMaster>();
    }

    public void OnStartFinish() {
      sceneMaster.StartFinish();
    }
  }
}

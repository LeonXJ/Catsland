using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Misc {

  public class Initializer : MonoBehaviour {
    // Start is called before the first frame update

    [Header("Player Position")]
    public bool setPlayerPositionOnStart = true;
    public Transform initPlayerPosition;

    void Start() {
      if (setPlayerPositionOnStart) {
        SceneConfig.getSceneConfig().player.transform.position = initPlayerPosition.position;
      }
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class ScoreUI : MonoBehaviour {

    private Text text;

    private void Awake() {
      text = GetComponent<Text>();
    }

    void Update() {
      GameObject player = SceneConfig.getSceneConfig().GetPlayer();
      int score = player.GetComponent<PlayerController>().score;

      text.text = "Score: " + score;
    }
  }
}

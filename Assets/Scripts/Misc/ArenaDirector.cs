using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;


namespace Catsland.Scripts.Misc {
  public class ArenaDirector : MonoBehaviour {

    private bool roundStarted = false;
    private HashSet<GameObject> livingOpponents = new HashSet<GameObject>();
    private int currentStage = -1;

    public bool autoStartFirstStage = true;
    public Text stageText;

    public PlayableDirector[] stages;

    // Start is called before the first frame update
    void Start() {
      if (autoStartFirstStage && stages.Length > 0) {
        currentStage = 0;
        stageText.text = "Stage " + (currentStage + 1);
        stages[currentStage].Play();
      }
    }

    // Update is called once per frame
    void Update() {
      if (roundStarted) {
        livingOpponents.RemoveWhere(g => g == null);
        if (livingOpponents.Count == 0) {
          // stage clear, next stage.
          currentStage++;
          if (currentStage < stages.Length) {
            stageText.text = "Stage " + (currentStage + 1);
            stages[currentStage].Play();
          }
        }
      }
    }

    public void StartRound() {
      roundStarted = true;
    }

    public void AddOpponent(GameObject gameObject) {
      livingOpponents.Add(gameObject);
    }
  }
}

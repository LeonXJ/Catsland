using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Catsland.Scripts.Misc {
  public class ArenaDirector : MonoBehaviour {

    private bool roundStarted = false;
    private HashSet<GameObject> livingOpponents = new HashSet<GameObject>();
    private int currentStage = -1;

    public bool autoStartFirstStage = true;
    public Text stageText;

    public StageConfig[] stageConfigs;

    // Start is called before the first frame update
    void Start() {
      if (autoStartFirstStage) {
        PlayFromBeginning();
      }
    }

    public void PlayFromBeginning() {
      if (stageConfigs.Length > 0) {
        currentStage = 0;
        initStage(stageConfigs[currentStage]);
      }
    }

    // Update is called once per frame
    void Update() {
      if (roundStarted) {
        livingOpponents.RemoveWhere(g => g == null);
        if (livingOpponents.Count == 0) {
          // stage clear, next stage.
          currentStage++;
          if (currentStage < stageConfigs.Length) {
            initStage(stageConfigs[currentStage]);
          }
          roundStarted = false;
        }
      }
    }

    public void StartRound() {
      roundStarted = true;
    }

    private void initStage(StageConfig stageConfig) {
      stageText.text = stageConfig.stageName;
      foreach(GameObject gameObject in stageConfig.opponents) {
        livingOpponents.Add(gameObject);
      }
      stageConfig.stageStartDirector?.Play();
    }
  }
}

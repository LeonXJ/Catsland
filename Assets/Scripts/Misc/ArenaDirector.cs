using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.Misc {
  public class ArenaDirector : MonoBehaviour {

    private bool roundStarted = false;
    private HashSet<GameObject> livingOpponents = new HashSet<GameObject>();
    public int currentStage = -1;

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
      currentStage = -1;
      PlayNextStage();
    }

    // Update is called once per frame
    void Update() {
      if (Input.GetKeyDown(KeyCode.F8)) {
        Debug.Log("Code input: kill all in stage# " + currentStage);
        foreach (GameObject opponent in livingOpponents) {
          if (opponent!= null) {
            opponent.SendMessage(
              Common.MessageNames.DAMAGE_FUNCTION,
              new DamageInfo(99999, opponent.transform.position, Vector2.up, 100f),
              SendMessageOptions.DontRequireReceiver);
          }
        }
      }

      if (roundStarted) {
        livingOpponents.RemoveWhere(g => g == null);
        if (livingOpponents.Count == 0) {
          roundStarted = false;
          // play end stage animation if have
          if (stageConfigs[currentStage].stageEndDirector != null) {
            stageConfigs[currentStage].stageEndDirector.Play();
          } else {
            PlayNextStage();
          }
        }
      }
    }

    public void PlayNextStage() {
      Debug.Log("PlayNextStage called. Current stage# " + currentStage);
      currentStage++;
      while (currentStage < stageConfigs.Length) {
        if (!stageConfigs[currentStage].skip) {
          initStage(stageConfigs[currentStage]);
          return;
        }
        currentStage++;
      }
    }

    public void StartRound() {
      roundStarted = true;
    }

    private void initStage(StageConfig stageConfig) {
      Debug.Log("Init stage: " + stageConfig.stageName);
      stageText.text = stageConfig.stageName;
      foreach(GameObject gameObject in stageConfig.opponents) {
        livingOpponents.Add(gameObject);
      }
      stageConfig.stageStartDirector?.Play();
    }
  }
}

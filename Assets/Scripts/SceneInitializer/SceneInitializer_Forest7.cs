using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Progress;

namespace Catsland.Scripts.SceneInitializer {
  public class SceneInitializer_Forest7 : SpecificSceneInitializerBase {

    public GameObject fakeBanditGo;
    public GameObject bossFightTriggerGo;

    public override void process() {
      bool? hasBanditDefeat = GameProgress.getInstance().getBoolProgress(GameProgress.ProgressKey.DEFEAT_BANDIT);
      if (hasBanditDefeat.GetValueOrDefault(false)) {
        fakeBanditGo.SetActive(false);
        bossFightTriggerGo.SetActive(false);
      }
    }
  }
}

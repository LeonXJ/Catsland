using UnityEngine;

namespace Catsland.Scripts.Misc {
  public class SceneConfig: MonoBehaviour {

    private static SceneConfig sceneConfig;

    public UnityEngine.Camera MainCamera;

    public static SceneConfig getSceneConfig() {
      return sceneConfig;
    }

    SceneConfig() {
      sceneConfig = this;
    }
  }
}

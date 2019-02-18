using UnityEngine;
using Catsland.Scripts.Camera;

namespace Catsland.Scripts.Common {
  public class SceneConfig: MonoBehaviour {

    private static SceneConfig sceneConfig;

    public UnityEngine.Camera MainCamera;
    public GlobalLightController globalLightController;

    public GameObject player;

    public static SceneConfig getSceneConfig() {
      return sceneConfig;
    }

    SceneConfig() {
      sceneConfig = this;
    }

    public GameObject GetPlayer() {
      if(player == null) {
        player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      }
      return player;
    }
  }
}

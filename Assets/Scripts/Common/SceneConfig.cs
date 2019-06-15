using UnityEngine;
using Catsland.Scripts.Camera;
using Catsland.Scripts.Misc;

namespace Catsland.Scripts.Common {
  public class SceneConfig: MonoBehaviour {

    private static SceneConfig sceneConfig;

    public UnityEngine.Camera MainCamera;
    public GlobalLightController globalLightController;
    public ProgressManager progressManager;
    public Shader defaultDiffuseShader;

    public GameObject player;

    private CameraController cameraController;

    public static SceneConfig getSceneConfig() {
      return sceneConfig;
    }

    private void Awake() {
      cameraController = MainCamera.GetComponent<CameraController>();
    }

    SceneConfig() {
      sceneConfig = this;
      progressManager = new ProgressManager();
    }

    public GameObject GetPlayer() {
      if(player == null) {
        player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      }
      return player;
    }

    public ProgressManager getProgressManager() {
      return progressManager;
    }

    public CameraController GetCameraController() {
      return cameraController;
    }

    public Shader GetDefaultDiffuseShader() {
      return defaultDiffuseShader;
    }
  }
}

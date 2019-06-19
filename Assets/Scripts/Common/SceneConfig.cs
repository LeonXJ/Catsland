using UnityEngine;
using Cinemachine;
using Catsland.Scripts.Camera;
using Catsland.Scripts.Misc;

namespace Catsland.Scripts.Common {
  public class SceneConfig: MonoBehaviour {

    private static SceneConfig sceneConfig;

    public UnityEngine.Camera MainCamera;
    public GlobalLightController globalLightController;
    public ProgressManager progressManager;
    public Shader defaultDiffuseShader;
    public FocusPointController cameraController;
    public CinemachineImpulseSource stonePillarShakeAgent;

    public GameObject player;


    public static SceneConfig getSceneConfig() {
      return sceneConfig;
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

    public FocusPointController GetCameraController() {
      return cameraController;
    }

    public Shader GetDefaultDiffuseShader() {
      return defaultDiffuseShader;
    }

    public void stonePillarShake(Vector3 position, Vector3 velocity) {
      Debug.Log("Shake");
      stonePillarShakeAgent.GenerateImpulseAt(position, velocity);
    }
  }
}

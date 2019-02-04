using UnityEngine;
using Catsland.Scripts.Camera;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class LightEffectArea: MonoBehaviour {

    public GlobalLightConfig globalColorConfig;

    private GlobalLightController globalLightController;

    public void Awake() {
      globalLightController = GameObject.FindGameObjectWithTag(Tags.SCENE_CONFIG)
        .GetComponent<SceneConfig>()
        .globalLightController;
    }

    public void OnTriggerEnter2D(Collider2D collision) {
      if(collision.CompareTag(Tags.PLAYER)) {
        globalLightController.RegisterColor(globalColorConfig);
      }
    }

    public void OnTriggerExit2D(Collider2D collision) {
      if(collision.CompareTag(Tags.PLAYER)) {
        globalLightController.UnregisterColor(globalColorConfig);
      }
    }
  }
}

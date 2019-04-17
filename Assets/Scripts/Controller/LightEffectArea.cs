using UnityEngine;
using Catsland.Scripts.Camera;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class LightEffectArea: MonoBehaviour {

    public GlobalLightConfig globalColorConfig;

    // To be refactor to multiple controller and config pairs
    public StackEffectController stackEffectController;
    public StackEffectConfig stackEffectConfig;

    private GlobalLightController globalLightController;

    public void Awake() {
      globalLightController = GameObject.FindGameObjectWithTag(Tags.SCENE_CONFIG)
        .GetComponent<SceneConfig>()
        .globalLightController;
    }

    public void OnTriggerEnter2D(Collider2D collision) {
      if(collision.CompareTag(Tags.PLAYER)) {
        globalLightController.RegisterColor(globalColorConfig);
        if(stackEffectController != null & stackEffectConfig != null) {
          stackEffectController.RegisterColor(stackEffectConfig);
        }
      }
    }

    public void OnTriggerExit2D(Collider2D collision) {
      if(collision.CompareTag(Tags.PLAYER)) {
        globalLightController.UnregisterColor(globalColorConfig);
        if(stackEffectController != null && stackEffectConfig != null) {
          stackEffectController.UnregisterColor(stackEffectConfig);
        }
      }
    }
  }
}

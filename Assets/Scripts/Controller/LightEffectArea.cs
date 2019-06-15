using UnityEngine;
using Catsland.Scripts.Camera;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class LightEffectArea: MonoBehaviour {

    public GlobalLightConfig globalColorConfig;

    // To be refactor to multiple controller and config pairs
    public SpriteColorStackEffectController spriteColorStackEffectController;
    public SpriteColorConfig spriteColorConfig ;

    private GlobalLightController globalLightController;

    public void Awake() {
      globalLightController = GameObject.FindGameObjectWithTag(Tags.SCENE_CONFIG)
        .GetComponent<SceneConfig>()
        .globalLightController;
    }

    public void OnTriggerEnter2D(Collider2D collision) {
      if(collision.CompareTag(Tags.PLAYER)) {
        if (globalColorConfig != null) {
          globalLightController.RegisterColor(globalColorConfig);
        }
        if(spriteColorStackEffectController != null & spriteColorConfig!= null) {
          spriteColorStackEffectController.RegisterColor(spriteColorConfig);
        }
      }
    }

    public void OnTriggerExit2D(Collider2D collision) {
      if(collision.CompareTag(Tags.PLAYER)) {
        if (globalColorConfig != null) {
          globalLightController.UnregisterColor(globalColorConfig);
        }
        if(spriteColorStackEffectController != null && spriteColorConfig!= null) {
          spriteColorStackEffectController.UnregisterColor(spriteColorConfig);
        }
      }
    }
  }
}

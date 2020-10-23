using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Camera {
  public class CameraOffsetArea : MonoBehaviour {
    public CameraOffsetConfig cameraOffsetConfig;

    public void OnTriggerEnter2D(Collider2D collision) {
      if (collision.CompareTag(Tags.PLAYER)) {
        SceneConfig.getSceneConfig().GetCameraController().RegisterConfig(cameraOffsetConfig);
      }
    }

    public void OnTriggerExit2D(Collider2D collision) {
      if (collision.CompareTag(Tags.PLAYER)) {
        SceneConfig.getSceneConfig().GetCameraController().UnregisterConfig(cameraOffsetConfig);
      }
    }

  }
}

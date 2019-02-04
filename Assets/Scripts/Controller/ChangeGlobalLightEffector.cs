using Catsland.Scripts.Camera;
using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class ChangeGlobalLightEffector: Effector {

    public Color targetAmbientColor;
    public GlobalLightController globalLightController;
    public float transistSpeed = 0.1f;

    public override void applyEffect() {
      // Deprecated
      globalLightController.backupAmbientColor =
        Color.Lerp(globalLightController.backupAmbientColor, targetAmbientColor, transistSpeed);
    }
  }
}

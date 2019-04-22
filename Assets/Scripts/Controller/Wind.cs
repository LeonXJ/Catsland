using UnityEngine;

namespace Catsland.Scripts.Controller {

  public class Wind : MonoBehaviour, IWind {

    public bool enable = true;
    public float windPower = 30.0f;

    // If true, the sign of wind will be flipped according to the transform's orientation.
    public bool useTransformDirection = true;

    public bool inverseTransformDirection = false;

    public float GetWindPower() {
      return enable
        ? (useTransformDirection ? (inverseTransformDirection ? -1.0f : 1.0f) * Mathf.Sign(transform.lossyScale.x) * windPower : windPower)
        : 0.0f;
    }
  }
}


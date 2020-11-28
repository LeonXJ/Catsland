using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Catsland.Scripts.Controller.Light {
  public class CoverSwitcher : MonoBehaviour {

    public Rect coverDetector;
    public LayerMask coverLayerMask;
    public float intensityChangeSpeedScale = 1f;

    private Light2D light2d;
    private float initLightIntensity;


    // Start is called before the first frame update
    void Start() {
      light2d = GetComponent<Light2D>();
      initLightIntensity = light2d.intensity;
      UpdateLight(true);
    }

    // Update is called once per frame
    void Update() {
      UpdateLight(false);
    }

    private void UpdateLight(bool immediateChange = false) {
      float targetIntensity = isCoverDetected() ? 0f : initLightIntensity;
      light2d.intensity = immediateChange
        ? targetIntensity
        : Mathf.Lerp(light2d.intensity, targetIntensity, intensityChangeSpeedScale * Time.deltaTime);
    }

    void OnDrawGizmosSelected() {
      Common.Utils.drawRectAsGizmos(coverDetector, isCoverDetected() ? Color.white : Color.yellow, transform);
    }
    private bool isCoverDetected() {
      return Common.Utils.isRectOverlap(coverDetector, transform, coverLayerMask);
    }
  }
}

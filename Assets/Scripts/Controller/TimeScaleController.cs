using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class TimeScaleController : StackEffectController<TimeScaleConfig> {

    private void Awake() {
      DontDestroyOnLoad(gameObject);
    }

    private void Update() {
      Time.timeScale = Mathf.Lerp(
        Time.timeScale,
        selectedConfig.timeScale,
        selectedConfig.valueChangeSpeed * Time.unscaledDeltaTime);
    }

    public bool RegisterConfig(TimeScaleConfig config) {
      return RegisterConfig(config, true);
    }
  }
}

using UnityEngine;

namespace Catsland.Scripts.Controller {
  [CreateAssetMenu(menuName = "ScriptableObjects/Config/TimeScaleConfig")]
  public class TimeScaleConfig: StackEffectConfig {
    public float timeScale;
  }
}

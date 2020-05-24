using UnityEngine;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Camera {

  [CreateAssetMenu(menuName = "ScriptableObjects/Config/GlobalLightConfig")]
  public class GlobalLightConfig: StackEffectConfig {
    public Color foregroundColor;
    public float foregroundIntensity;
    public Color backgroundColor;
    public float backgroundIntensity;
  }
}

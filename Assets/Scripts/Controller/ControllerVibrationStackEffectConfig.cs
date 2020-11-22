using UnityEngine;

namespace Catsland.Scripts.Controller {
  [CreateAssetMenu(menuName = "ScriptableObjects/Config/ControllerVibrationConfig")]
  public class ControllerVibrationStackEffectConfig : ScriptableObject {
    public string name;
    public float lowFrequency;
    public float highFrequency;
    public float lastSeconds;
    public bool deduplicate;
  }
}

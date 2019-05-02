using UnityEngine;

namespace Catsland.Scripts.Controller {
  public abstract class StackEffectConfig: MonoBehaviour {
    public string configName;
    public int priority;
    public float valueChangeSpeed;
  }
}

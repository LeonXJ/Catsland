using UnityEngine;

namespace Catsland.Scripts.Controller {
  public abstract class StackEffectConfig: ScriptableObject {
    public int priority;
    public float valueChangeSpeed;
  }
}

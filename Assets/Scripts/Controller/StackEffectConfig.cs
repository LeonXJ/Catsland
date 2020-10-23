using UnityEngine;

namespace Catsland.Scripts.Controller {
  public abstract class StackEffectConfig: ScriptableObject {
    public string channelName;
    public int priority;
    public float valueChangeSpeed;
  }
}

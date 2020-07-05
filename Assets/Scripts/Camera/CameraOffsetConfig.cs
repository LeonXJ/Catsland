using UnityEngine;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Camera {
  [CreateAssetMenu(menuName = "ScriptableObjects/Config/CameraOffsetConfig")]
  public class CameraOffsetConfig : StackEffectConfig {
    public Vector2 offset;
  }
}

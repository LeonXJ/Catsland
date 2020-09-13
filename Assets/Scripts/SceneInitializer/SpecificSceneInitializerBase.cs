using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.SceneInitializer {
  public abstract class SpecificSceneInitializerBase : MonoBehaviour {
    public abstract void process();
  }
}

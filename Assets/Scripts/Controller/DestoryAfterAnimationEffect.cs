using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class DestoryAfterAnimationEffect: MonoBehaviour {
    public void selfDestory() {
      Destroy(gameObject);
    }
  }
}

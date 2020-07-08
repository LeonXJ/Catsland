using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Misc {
  public class SceneTransitionTrigger : MonoBehaviour {

    public SceneMaster.SceneTransitInfo transiteInfo;

    private void OnTriggerEnter2D(Collider2D collision) {
      if (collision.gameObject.CompareTag(Common.Tags.PLAYER)) {
        SceneMaster.getInstance().TransitionToScene(transiteInfo);
      }
    }
  }
}

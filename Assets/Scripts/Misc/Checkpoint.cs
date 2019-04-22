using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Misc {

  [RequireComponent(typeof(Collider2D))]
  public class Checkpoint : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
      if (collision.gameObject.CompareTag(Tags.PLAYER)) {
        Debug.Log("Progress Saved.");
        SceneConfig.getSceneConfig().getProgressManager().Save(
          ProgressManager.Progress.Create(collision.gameObject));
      }
    }
  }
}



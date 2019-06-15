using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Misc {

  [RequireComponent(typeof(Collider2D))]
  public class Checkpoint : MonoBehaviour {
    private const string IS_LIT = "IsLit";

    private Animator animator;

    private void Awake() {
      animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
      if (collision.gameObject.CompareTag(Tags.PLAYER)) {
        Debug.Log("Progress Saved.");
        SceneConfig.getSceneConfig().getProgressManager().Save(
          ProgressManager.Progress.Create(collision.gameObject));

        if (animator != null) {
          animator.SetBool(IS_LIT, true);
        }
      }
    }
  }
}



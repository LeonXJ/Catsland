using Catsland.Scripts.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Misc {
  public class CheckpointRegister : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
      if (collision.gameObject.CompareTag(Tags.PLAYER)) {
        SceneInitializer sceneInitializer = FindObjectOfType<SceneInitializer>();
        Debug.Assert(sceneInitializer != null, "The scene doesn't have SceneInitializer.");

        sceneInitializer.registerCheckpoint(transform);
      }
    }
  }
}

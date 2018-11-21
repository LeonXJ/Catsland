using UnityEngine;

using Catsland.Scripts.Misc;

namespace Catsland.Scripts.Controller {
  public class PlayerTrigger: MonoBehaviour {
    public GameObject[] effectGameObjects;

    public void OnTriggerStay2D(Collider2D collision) {
      if(collision.gameObject == SceneConfig.getSceneConfig().player) {
        foreach(GameObject go in effectGameObjects) {
          foreach(Effector effector in go.GetComponentsInChildren<Effector>()) {
            effector.applyEffect();
          }
        }
      }
    }
  }
}

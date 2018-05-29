using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class PlayerTrigger :MonoBehaviour {

    public GameObject triggerGameObject;
    public GameObject[] effectGameObjects;

    public void OnTriggerStay2D(Collider2D collision) {
      if(collision.gameObject == triggerGameObject) {
        foreach(GameObject go in effectGameObjects) {
          foreach(Effector effector in go.GetComponentsInChildren<Effector>()) {
            effector.applyEffect();
          }
        }
      }
    }
  }
}

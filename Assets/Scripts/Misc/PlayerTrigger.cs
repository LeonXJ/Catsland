using UnityEngine;

public class PlayerTrigger :MonoBehaviour {

  public GameObject triggerGameObject;
  public GameObject[] effectGameObjects;

  public void OnTriggerStay2D(Collider2D collision) {
    if(collision.gameObject == triggerGameObject) {
      foreach(GameObject go in effectGameObjects) {
        foreach(ColorAdjustEffector effector in go.GetComponentsInChildren<ColorAdjustEffector>()) {
          effector.applyEffect();
        }
      }
    }
  }
}

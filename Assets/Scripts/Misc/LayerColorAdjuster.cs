using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Catsland.Scripts.Misc {
  [RequireComponent(typeof(Collider2D))]
  public class LayerColorAdjuster :MonoBehaviour {

    public GameObject triggerGameObject;
    public GameObject[] layerGameObjects;
    public Color targetColor;

    public void OnTriggerStay2D(Collider2D collision) {
      if(collision.gameObject == triggerGameObject) {
        foreach(GameObject go in layerGameObjects) {
          foreach(SpriteRenderer renderer in 
              go.GetComponentsInChildren<SpriteRenderer>()) {
            renderer.color = Color.Lerp(renderer.color, targetColor, 0.1f);
          }
        }
      }
    }
  }
}

using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Misc {
  [RequireComponent(typeof(Collider2D))]
  public class LayerAdjuster :MonoBehaviour {

    public GameObject triggerGameObject;
    public GameObject gameObjectToBeAdjusted;
    public int adjustOffset;

    private Dictionary<SpriteRenderer, int> orginalOrder;

    // Use this for initialization
    void Start() {
      if(gameObjectToBeAdjusted == null) {
        return;
      }

      SpriteRenderer[] renderers = gameObjectToBeAdjusted.GetComponentsInChildren<SpriteRenderer>();
      orginalOrder = new Dictionary<SpriteRenderer, int>();
      foreach(SpriteRenderer renderer in renderers) {
        orginalOrder.Add(renderer, renderer.sortingOrder);
      }
    }

    public void OnTriggerStay2D(Collider2D collision) {
      if(collision.gameObject == triggerGameObject) {
        adjustOrder();
      }
    }

    private void adjustOrder() {
      if(orginalOrder == null) {
        return;
      }
      foreach(KeyValuePair<SpriteRenderer, int> entry in orginalOrder) {
        entry.Key.sortingOrder = entry.Value + adjustOffset;
      }
    }
  }
}

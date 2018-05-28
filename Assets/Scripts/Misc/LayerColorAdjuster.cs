using UnityEngine;


namespace Catsland.Scripts.Misc {
  [RequireComponent(typeof(Collider2D))]
  public class LayerColorAdjuster :MonoBehaviour {

    public GameObject triggerGameObject;
    public GameObject[] layerGameObjects;
    public Color targetColor;
    public Color targetAmbientLight;

    public void OnTriggerStay2D(Collider2D collision) {
      if(collision.gameObject == triggerGameObject) {
        foreach(GameObject go in layerGameObjects) {
          foreach(SpriteRenderer renderer in
              go.GetComponentsInChildren<SpriteRenderer>()) {
            if(renderer.material.name == "LightSprite (Instance)") {
              renderer.material.SetColor("_Color", Color.Lerp(renderer.material.GetColor("_Color"), targetColor, 0.1f));
              renderer.material.SetColor("_AmbientLight", Color.Lerp(renderer.material.GetColor("_AmbientLight"), targetAmbientLight, 0.1f));
            }
          }
        }
      }
    }
  }
}

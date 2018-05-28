using UnityEngine;

public class ColorAdjustEffector :MonoBehaviour {
  public GameObject[] layerGameObjects;
  public Color targetColor;
  public Color targetAmbientLight;

  public void applyEffect() {
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

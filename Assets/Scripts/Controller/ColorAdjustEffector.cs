using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class ColorAdjustEffector :Effector {
    public GameObject[] layerGameObjects;
    public Color targetColor;
    public Color targetAmbientLight;
    public float transistSpeed = 0.1f;

    public override void applyEffect() {
      foreach(GameObject go in layerGameObjects) {
        foreach(SpriteRenderer renderer in
            go.GetComponentsInChildren<SpriteRenderer>()) {
          if(renderer.material.name == "DiffuseSprite (Instance)") {
            renderer.material.SetColor("_Color", Color.Lerp(renderer.material.GetColor("_Color"), targetColor, transistSpeed));
            renderer.material.SetColor("_AmbientLight", Color.Lerp(renderer.material.GetColor("_AmbientLight"), targetAmbientLight, transistSpeed));
          }
        }
      }
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Fx {

  [ExecuteInEditMode]
  public class HueShift : MonoBehaviour {

    public Color hueShift;

    private new SpriteRenderer renderer;

    void Start() {
      renderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update() {
      if (renderer.material.name == "DiffuseSprite (Instance)") {
        renderer.material.SetColor("_HueShift", hueShift);
      }
    }
  }
}

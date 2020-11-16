using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Misc {
  public class AutoUnhide : MonoBehaviour {

    private void Start() {
      Unhide();
    }

    public void Unhide() {
      Renderer renderer = GetComponent<Renderer>();
      if (renderer != null) {
        renderer.enabled = true;
      }
    }
  }
}

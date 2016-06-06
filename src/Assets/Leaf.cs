using UnityEngine;
using System.Collections;
using System;

namespace Catslandx {
  public class Leaf :MonoBehaviour, IObjectDisposalReceiver {

    public GameObject leafRoot;
    public float selfDestroyHeight = -10.0f;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
      if (transform.position.y < selfDestroyHeight) {
        selfDestroy();
      }
    }

    private void selfDestroy() {
      DestroyObject(leafRoot);
    }

    bool IObjectDisposalReceiver.selfDestroy() {
      selfDestroy();
      return true;
    }
  }
}

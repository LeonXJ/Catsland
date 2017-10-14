using UnityEngine;
using System.Collections;

namespace Catslandx {
  public class ObjectDisposal :MonoBehaviour {
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter2D(Collider2D other) {
      IObjectDisposalReceiver disposal = other.gameObject.GetComponent<IObjectDisposalReceiver>();
      if (disposal == null || !disposal.selfDestroy()) {
        DestroyObject(other.gameObject);
      }
    }
  }
}

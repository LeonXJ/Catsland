using UnityEngine;
using System.Collections;

namespace Catslandx {
  public class SelfDestroy :MonoBehaviour {

    public float lifeInS = 5.0f;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
      if (lifeInS > 0.0f) {
        lifeInS -= Time.deltaTime;
      } else {
        Destroy(gameObject);
      }
    }
  }
}

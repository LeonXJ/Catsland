using UnityEngine;
using System.Collections;

namespace Catslandx {
  public class RespawnSensor :MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter2D(Collider2D other) {
      IRespawn respawn = other.gameObject.GetComponent<IRespawn>();
      if (respawn != null) {
        respawn.setRespawn(gameObject);
      }
    }
  }
}

using UnityEngine;
using Catslandx.Script.Common;

namespace Catslandx {
  [RequireComponent(typeof(Collider2D))]
  public class Thorn :MonoBehaviour {

    public int hurtPoint = 1;
    public float repelScale = 1.0f;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter2D(Collider2D other) {
      IVulnerable vulnerable = other.gameObject.GetComponent<IVulnerable>();
      if (vulnerable != null) {
        Vector2 repelForce = (other.gameObject.transform.position - transform.position).normalized * repelScale;
        vulnerable.getHurt(hurtPoint, repelForce);
      }
    }

  }
}


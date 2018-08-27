using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class Follower: MonoBehaviour {
    public GameObject target;

    void Update() {
      transform.position = target.transform.position;
    }
  }
}

using UnityEngine;

namespace Catsland.Scripts.Physics {
  public class Flame :MonoBehaviour {
    public int intensity = 1;

    void OnTriggerStay2D(Collider2D other) {
      if(gameObject == other.gameObject) {
        return;
      }
      IIgnitable ignitable = other.gameObject.GetComponent<IIgnitable>();
      if(ignitable != null) {
        ignitable.heat(intensity);
      }
    }
  }
}

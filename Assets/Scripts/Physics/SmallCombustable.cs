using UnityEngine;

namespace Catsland.Scripts.Physics {
  [RequireComponent(typeof(Collider2D))]
  public class SmallCombustable: MonoBehaviour, IIgnitable {

    public GameObject flamePrefab;
    public Transform flamePoint;
    public int igniteIntensity = 1;

    // Use float considering cool down.
    public float currentIntensity = 0;

    public GameObject flame;

    public void heat(int intensity) {
      if(flame == null) {
        currentIntensity += intensity;
        if(currentIntensity > igniteIntensity) {
          ignite();
        }
      }
    }

    public void ignite() {
      flame = Instantiate(flamePrefab);
      if(flamePoint != null) {
        flame.transform.position = flamePoint.position;
      } else {
        flame.transform.position = gameObject.transform.position;
      }
      flame.transform.parent = gameObject.transform;
    }

    // Update is called once per frame
    void Update() {
      // TODO: implement cooldown

    }
  }
}

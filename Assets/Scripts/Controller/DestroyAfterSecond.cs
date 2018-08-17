using System.Collections;
using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class DestroyAfterSecond: MonoBehaviour {

    public float lifetime = 1.0f;
    public bool manualStart = false;

    void Start() {
      if(!manualStart) {
        startTimer();
      }
    }

    public void startTimer() {
      StartCoroutine(destoryAfterSecond(lifetime));
    }

    private IEnumerator destoryAfterSecond(float second) {
      yield return new WaitForSeconds(second);
      Destroy(gameObject);
    }
  }
}

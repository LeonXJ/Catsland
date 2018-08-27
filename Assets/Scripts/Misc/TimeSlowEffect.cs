using System.Collections;
using UnityEngine;

namespace Catsland.Scripts.Misc {
  public class TimeSlowEffect: MonoBehaviour {

    public float slowScale = 0.2f;
    public float duration = 0.2f;

    public void slow() {
      Time.timeScale = slowScale;
      StartCoroutine(waitAndRecover());
    }

    private IEnumerator waitAndRecover() {
      yield return new WaitForSeconds(duration);
      recover();
    }

    public void recover() {
      Time.timeScale = 1.0f;
    }
  }
}

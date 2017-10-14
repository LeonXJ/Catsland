using UnityEngine;
using System.Collections;

namespace Catslandx {
  public class TrailRenderController :MonoBehaviour {

    public float appearSpeed = 1.0f;
    public float disappearSpeed = 0.5f;
    public float disableThreshold = 0.01f;

    private TrailRenderer trailRender;
    public float targetLengthSecond = 0.0f;

    // Use this for initialization
    void Start() {
      trailRender = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update() {
      if (trailRender == null) {
        return;
      }
      // disappear
      if (trailRender.enabled && trailRender.time > targetLengthSecond) {
        float nextLength = trailRender.time - Time.deltaTime * disappearSpeed;
        if (nextLength < targetLengthSecond) {
          nextLength = targetLengthSecond;
        }
        trailRender.time = nextLength;
        // disable?
        if (nextLength < disableThreshold) {
          trailRender.enabled = false;
        }
      } else {
        // appear
        float currentLength = trailRender.enabled ? trailRender.time : 0.0f;
        float nextLength = currentLength + Time.deltaTime * appearSpeed;
        if (nextLength > targetLengthSecond) {
          nextLength = targetLengthSecond;
        }
        if (!trailRender.enabled) {
          trailRender.enabled = true;
        }
        trailRender.time = nextLength;
      }
    }
  }
}
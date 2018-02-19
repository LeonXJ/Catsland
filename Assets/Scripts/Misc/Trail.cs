using UnityEngine;

namespace Catsland.Scripts.Misc {
  public class Trail :MonoBehaviour {

    public bool isShow = true;
    public float hideSpeed = 1.0f;

    private TrailRenderer trailRenderer;

    private void Awake() {
      trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update() {
      if(!isShow) {
        trailRenderer.time = Mathf.Lerp(trailRenderer.time, 0.0f, Time.deltaTime * hideSpeed);
      }
    }
  }



}

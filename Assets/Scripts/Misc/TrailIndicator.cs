using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Misc {
  public class TrailIndicator :MonoBehaviour {

    public float initVelocity = 1.0f;
    public float timeInterval = 0.1f;
    public float gravity = -0.98f;
    public bool isShow = false;
    public float showTransitSpeed = 5.0f;
    public float hideTransitSpeed = 16.0f;
    public float maxAlpha = 0.3f;

    private LineRenderer lineRenderer;
    private float alpha;

    private void Awake() {
      lineRenderer = GetComponent<LineRenderer>();
    }
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

      alpha = Mathf.Lerp(
        alpha,
        isShow ? maxAlpha : 0.0f,
        Time.deltaTime * (isShow ? showTransitSpeed : hideTransitSpeed));
      lineRenderer.material.SetColor("_TintColor", new Color(1.0f, 1.0f, 1.0f, alpha));

      if(alpha > Mathf.Epsilon) {
        int pointCount = lineRenderer.positionCount;
        Vector3[] positions = new Vector3[pointCount];
        lineRenderer.GetPositions(positions);

        for(int i = 0; i < pointCount; i++) {
          float x = initVelocity * timeInterval * i;
          float y = 0.5f * gravity * timeInterval * timeInterval * i * i;
          positions[i] = new Vector3(x, y, 0.0f);
        }

        lineRenderer.SetPositions(positions);
      }
    }
  }
}

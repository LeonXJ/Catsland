using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Misc {
  public class TrailIndicator :MonoBehaviour {

    public Vector2 initVelocity = new Vector2(1.0f, 0.0f);
    public float timeInterval = 0.1f;
    public float shootingRange = 3f;
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

    // Update is called once per frame
    void Update() {

      alpha = Mathf.Lerp(
        alpha,
        isShow ? maxAlpha : 0.0f,
        Time.deltaTime * (isShow ? showTransitSpeed : hideTransitSpeed));
      lineRenderer.material.SetColor(Common.Materials.MATERIAL_ATTRIBUTE_TINT, new Color(1.0f, 1.0f, 1.0f, alpha));

      // absolote length: 0 - 1 - 5
      // alpha: 0 - 1 - 0


      if(alpha > Mathf.Epsilon) {
        int pointCount = lineRenderer.positionCount;
        Vector3[] positions = new Vector3[pointCount];
        lineRenderer.GetPositions(positions);

        Vector2 originalInWorld = Common.Utils.toVector2(transform.position);
        Vector2 directionInWorld = transform.TransformDirection(initVelocity);
        RaycastHit2D hit = Physics2D.Raycast(
          originalInWorld, 
          directionInWorld.normalized,
          shootingRange, 
          LayerMask.GetMask(Common.Layers.LAYER_GROUND_NAME));

        positions[0] = Vector2.zero;
        if (hit) {
          positions[1] = transform.InverseTransformPoint(hit.point);
        } else {
          positions[1] = new Vector2(shootingRange, 0f);
        }
        lineRenderer.SetPositions(positions);
      }
    }
  }
}

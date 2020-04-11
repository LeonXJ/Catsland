using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller.Slime {
  [ExecuteInEditMode]
  public class SlimeStoneBend : MonoBehaviour {
    public GameObject leftShield;
    public Vector3 leftShieldPointOffset;
    public GameObject rightShield;
    public Vector3 rightShieldPointOffset;
    public Color bendColor;

    public Transform centerPoint;

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start() {
      lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update() {
      lineRenderer.SetPosition(0, leftShield.transform.localPosition + leftShieldPointOffset);
      lineRenderer.SetPosition(1, centerPoint.localPosition);
      lineRenderer.SetPosition(2, rightShield.transform.localPosition + rightShieldPointOffset);
      lineRenderer.startColor = bendColor;
      lineRenderer.endColor = bendColor;
    }
  }
}

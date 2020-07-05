using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Physics {
  [ExecuteInEditMode]
  public class Rope : MonoBehaviour {

    public HingeJoint2D finalHinge;

    [Range(2, 30)]
    public int segmentCount = 2;

    [Range(.1f, 1f)]
    public float maxSegmentLength = .2f;

    public List<HingeJoint2D> hinges;
    public float ropeDensity = .1f;

    private LineRenderer lineRenderer;

    void Start() {
      lineRenderer = GetComponent<LineRenderer>();

    }

    void Update() {
      int hingeNum = hinges == null ? 0 : hinges.Count;
      if (hingeNum == 0) {
        lineRenderer.positionCount = 0;
        return;
      }
      if (lineRenderer.positionCount != hingeNum + 1) {
        lineRenderer.positionCount = hingeNum + 1;
      }
      lineRenderer.SetPosition(0, hinges[0].connectedBody.transform.TransformPoint(hinges[0].connectedAnchor));
      for (int i = 0; i < hingeNum; i++) {
        lineRenderer.SetPosition(i + 1, hinges[i].transform.TransformPoint(hinges[i].anchor));
      }
    }
  }
}

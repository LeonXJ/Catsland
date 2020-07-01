using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Physics {
  [ExecuteInEditMode]
  public class Rope : MonoBehaviour {

    public List<HingeJoint2D> hinges;

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
      if (lineRenderer.positionCount != hingeNum) {
        lineRenderer.positionCount = hingeNum + 1;
      }
      lineRenderer.SetPosition(0, hinges[0].connectedBody.transform.TransformPoint(hinges[0].connectedAnchor));
      for (int i = 1; i < hingeNum; i++) {
        lineRenderer.SetPosition(i, hinges[i].transform.TransformPoint(hinges[i].anchor));
      }
    }
  }
}

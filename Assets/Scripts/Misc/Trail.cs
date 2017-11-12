using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Misc {
  public class Trail :MonoBehaviour {

    public bool isShow;

    private LineRenderer lineRenderer;

    private void Awake() {
      lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update() {

    }
  }



}

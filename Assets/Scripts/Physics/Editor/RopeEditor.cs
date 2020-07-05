using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEditor.Experimental.GraphView;

namespace Catsland.Scripts.Physics.Editor {
  [CustomEditor(typeof(Rope))]
  public class RopeEditor : UnityEditor.Editor {

    public static readonly string HINGE_CHILD_NAME_PREFIX = "GenHinge_";

    public override void OnInspectorGUI() {
      base.OnInspectorGUI();

      Rope rope = target as Rope;
      if (GUILayout.Button("Create Hinges")) {
        // Clean exisring Hinges
        ArrayList toBeDestory = new ArrayList();
        for (int i = 0; i < rope.transform.childCount; i++) {
          GameObject child = rope.transform.GetChild(i).gameObject;
          if (child.name.StartsWith(HINGE_CHILD_NAME_PREFIX)) {
            toBeDestory.Add(child);
          }
        }
        foreach (GameObject child in toBeDestory) {
          DestroyImmediate(child);
        }

        // Create Hinges, in world space
        rope.hinges.Clear();
        Vector3 start = rope.transform.position;
        Vector3 end = rope.finalHinge.transform.TransformPoint(rope.finalHinge.anchor);
        float ropeLength = (end - start).magnitude;
        int refinedSegmentCount = Mathf.Max(rope.segmentCount, (int) Mathf.Ceil(ropeLength / rope.maxSegmentLength));
        float segmentMass = rope.ropeDensity * ropeLength / refinedSegmentCount;

        Rigidbody2D preRb2d = rope.GetComponent<Rigidbody2D>();
        for (int i = 0; i < refinedSegmentCount - 1; i++) {
          Vector3 pos = Vector3.Lerp(start, end, ((float)(i + 1)) / refinedSegmentCount);

          GameObject child = new GameObject(HINGE_CHILD_NAME_PREFIX + i);
          child.transform.SetParent(rope.transform);
          child.transform.position = pos;

          Rigidbody2D childRb2d = child.AddComponent<Rigidbody2D>();
          childRb2d.mass = segmentMass;
          HingeJoint2D childHinge = child.AddComponent<HingeJoint2D>();
          if (i == 0) {
            childHinge.autoConfigureConnectedAnchor = false;
            childHinge.connectedAnchor = Vector2.zero;
          }
          childHinge.connectedBody = preRb2d;
          rope.hinges.Add(childHinge);

          preRb2d = childRb2d;
        }
        rope.finalHinge.connectedBody = preRb2d;
        rope.hinges.Add(rope.finalHinge);
      }
    }

  }
}

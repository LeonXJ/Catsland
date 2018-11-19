using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Catsland.Scripts.Camera.Editor {
  [CustomEditor(typeof(CameraController))]
  public class CameraControllerEditor: UnityEditor.Editor {
    public override void OnInspectorGUI() {
      base.OnInspectorGUI();

      CameraController cameraController = target as CameraController;
      if(GUILayout.Button("Set to Position")) {
        cameraController.setToTargetPosition();
      }
    }

  }
}

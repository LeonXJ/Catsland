using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Catsland.Scripts.Camera.Editor {
  [CustomEditor(typeof(FocusPointController))]
  public class CameraControllerEditor: UnityEditor.Editor {
    public override void OnInspectorGUI() {
      base.OnInspectorGUI();

      FocusPointController cameraController = target as FocusPointController;
      if(GUILayout.Button("Set to Position")) {
        //cameraController.setToTargetPosition();
      }
    }

  }
}

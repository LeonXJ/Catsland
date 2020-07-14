using UnityEngine;
using UnityEditor;

namespace Catsland.Scripts.Controller.Editor {
  [CustomEditor(typeof(GrassModel))]
  [CanEditMultipleObjects]
  public class GrassModelEditor: UnityEditor.Editor {

    public override void OnInspectorGUI() {
      base.OnInspectorGUI();

      GrassModel grassModel = target as GrassModel;
      if(GUILayout.Button("Set Size According to Texture")) {
        grassModel.initializeMesh();
        grassModel.UpdateSize();
        grassModel.OnAttributeUpdate();
      }
      if(GUILayout.Button("Apply All")) {
        grassModel.OnAttributeUpdate();
      }
    }

  }
}

using UnityEngine;
using UnityEditor;

namespace Catsland.Scripts.Controller.Editor {
  [CustomEditor(typeof(GrassModel))]
  [CanEditMultipleObjects]
  public class GrassModelEditor: UnityEditor.Editor {

    float overlapDistance = 0.01f;
    
    public override void OnInspectorGUI() {
      base.OnInspectorGUI();

      if(GUILayout.Button("Set Size According to Texture")) {
        foreach(GameObject go in Selection.gameObjects) {
          GrassModel grassModel = go.GetComponent<GrassModel>();
          grassModel.initializeMesh();
          grassModel.UpdateSize();
          grassModel.OnAttributeUpdate();
        }
      }
      if(GUILayout.Button("Apply All")) {
        foreach (GameObject go in Selection.gameObjects) {
          GrassModel grassModel = go.GetComponent<GrassModel>();
          grassModel.OnAttributeUpdate();
        }
      }

      overlapDistance = EditorGUILayout.FloatField(overlapDistance);
      if (GUILayout.Button("Land")) {
        foreach (GameObject go in Selection.gameObjects) {
          GrassModel grassModel = go.GetComponent<GrassModel>();
          grassModel.Land(5f, overlapDistance);
        }
      }
    }

  }
}

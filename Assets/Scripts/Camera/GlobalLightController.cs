using UnityEngine;

namespace Catsland.Scripts.Camera {
  [ExecuteInEditMode]
  public class GlobalLightController :MonoBehaviour {
    public Color ambient;

    private UnityEngine.Camera lightmapCamera;

    void Awake() {
      lightmapCamera = GetComponent<UnityEngine.Camera>();
    }

    private void Update() {
      if(lightmapCamera != null) {
        lightmapCamera.backgroundColor = ambient;
      }
    }
  }
}

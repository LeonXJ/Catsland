using UnityEngine;

public class ParentCameraMatcher :MonoBehaviour {
  private Camera cameraComponent;
  private Camera parentCamera;

  void Start() {
    cameraComponent = GetComponent<Camera>();
    parentCamera = transform.parent.GetComponent<Camera>();
  }

  void Update() {
    cameraComponent.fieldOfView = parentCamera.fieldOfView;
    cameraComponent.farClipPlane = parentCamera.farClipPlane;
  }
}

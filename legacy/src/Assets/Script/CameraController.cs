using UnityEngine;
using System.Collections;

namespace Catsland {
  public class CameraController : MonoBehaviour {

    public Transform target;

    public Vector2 edgeWidth = new Vector2(0.1f, 0.1f);

    private Camera targetCamera;

    // Use this for initialization
    void Start () {
      targetCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update() {
      if(target == null || targetCamera == null) {
        return;
      }

      float cameraHalfHeight = targetCamera.orthographicSize;
      float cameraHalfWidth = cameraHalfHeight * targetCamera.aspect;

      float targetCameraX = targetCamera.transform.position.x;
      float targetCameraY = targetCamera.transform.position.y;
      if (target.position.x < targetCamera.transform.position.x - cameraHalfWidth + cameraHalfWidth * edgeWidth.x * 2f) {
        targetCameraX = target.position.x + cameraHalfWidth - cameraHalfWidth * edgeWidth.x * 2f;
      } else if (target.position.x > targetCamera.transform.position.x + cameraHalfWidth - cameraHalfWidth * edgeWidth.x * 2f) {
        targetCameraX = target.position.x - cameraHalfWidth + cameraHalfWidth * edgeWidth.x * 2f;
      }
      if (target.position.y < targetCamera.transform.position.y - cameraHalfHeight + cameraHalfHeight * edgeWidth.y * 2f) {
        targetCameraY = target.position.y + cameraHalfHeight - cameraHalfHeight * edgeWidth.y * 2f;
      } else if (target.position.y > targetCamera.transform.position.y + cameraHalfHeight - cameraHalfHeight * edgeWidth.y * 2f) {
        targetCameraY = target.position.y - cameraHalfHeight + cameraHalfHeight * edgeWidth.y * 2f;
      }
      targetCamera.transform.position = new Vector3(targetCameraX, targetCameraY, targetCamera.transform.position.z);
    }
  }
}

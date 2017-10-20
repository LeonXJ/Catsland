using UnityEngine;

namespace Catsland.Scripts.Camera {
  public class CameraController :MonoBehaviour {

    public float horizontalSmooth;
    public float verticalSmooth;
    public GameObject target;

    private Vector2 velocity;

    // Update is called once per frame
    void Update() {
      if(target != null) {
        float newX = Mathf.SmoothDamp(
          transform.position.x, target.transform.position.x, ref velocity.x, horizontalSmooth);
        float newY = Mathf.SmoothDamp(
          transform.position.y, target.transform.position.y, ref velocity.y, verticalSmooth);

        transform.position = new Vector3(newX, newY, transform.position.z);

      }

    }
  }


}

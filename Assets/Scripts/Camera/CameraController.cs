using UnityEngine;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Camera {
  public class CameraController: StackEffectController<CameraOffsetConfig> {

    public float horizontalSmooth;
    public float verticalSmooth;
    public GameObject target;
    public Vector2 defaultOffset = Vector2.zero;
    public float defaultChangeSpeed = 0.2f;

    private Vector2 velocity;
    private Vector2 currentOffset = Vector2.zero;

    // Update is called once per frame
    void Update() {
      if(target != null) {

        if (topPrioritizedColor != null) {
          currentOffset = Vector2.Lerp(currentOffset, topPrioritizedColor.offset, topPrioritizedColor.valueChangeSpeed * Time.deltaTime);
        } else {
          currentOffset = Vector2.Lerp(currentOffset, defaultOffset, defaultChangeSpeed * Time.deltaTime);
        }

        float newX = Mathf.SmoothDamp(
          transform.position.x, target.transform.position.x + currentOffset.x , ref velocity.x, horizontalSmooth);
        float newY = Mathf.SmoothDamp(
          transform.position.y, target.transform.position.y + currentOffset.y, ref velocity.y, verticalSmooth);

        transform.position = new Vector3(newX, newY, transform.position.z);

      }
    }

    public void setToTargetPosition() {
      transform.position = new Vector3(
        target.transform.position.x, target.transform.position.y, transform.position.z);
    }
  }


}

using UnityEngine;

namespace Catsland.Scripts.Controller.Tortoise {

  [RequireComponent(typeof(Rigidbody2D))]
  public class TortoiseController : MonoBehaviour {

    public Rect frontSpaceDetector;
    public LayerMask frontDetectorMask;

    public float movingMaxSpeed = 2f;
    public float movingPropelForce = 10f;
    public float movingPropelInternal = 0.2f;
    public bool fixY = true;

    private float lastPropelTime = 0f;

    private Rigidbody2D rb2d;
    private float initialY = 0f;

    // Start is called before the first frame update
    void Start() {
      rb2d = GetComponent<Rigidbody2D>();
      initialY = transform.position.y;
    }

    void Update() {
      if (IsFrontObstacleDetected) {
        TurnAround();
      }
      if (lastPropelTime + movingPropelInternal < Time.time) {
        rb2d.AddForce(new Vector2(Orientation * movingPropelForce, 0f));
        lastPropelTime = Time.time;
        if (Mathf.Abs(rb2d.velocity.x) > movingMaxSpeed) {
          rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * movingMaxSpeed, rb2d.velocity.y);
        }
      }
      if (fixY) {
        transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
      }
    }

    void OnDrawGizmosSelected() {
      Common.Utils.drawRectAsGizmos(frontSpaceDetector, IsFrontObstacleDetected ? Color.red : Color.white, transform);
    }

    private void TurnAround() => transform.localScale = new Vector3(transform.localScale.x * -1f, 1f, 1f);

    private float Orientation => transform.lossyScale.x;

    private bool IsFrontObstacleDetected
      => Common.Utils.isRectOverlap(frontSpaceDetector, transform, frontDetectorMask);
      
  }
}

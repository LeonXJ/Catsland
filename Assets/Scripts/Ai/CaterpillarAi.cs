using UnityEngine;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ai {

  // TODO: create a common AI.
  public class CaterpillarAi: MonoBehaviour, CaterpillarController.CaterpillarInput {

    public LayerMask groundLayerMask;
    public Rect frontSpaceDetector;
    public Rect frontGroundDetector;
    public Rect belowGroundDetector;
    public GameObject playerSensorGo;
    public bool camelflagIfNoFoundPlayer = false;
    public bool camelflagAgain = false;
    public float timeToCamelflagInS = 5.0f;

    private float horizontalSpeed = 0.0f;
    private ISensor playerSensor;
    private bool hasFoundPlayer = false;
    private float timeToCamelflagTimeInS = 0.0f;

    private void Awake() {
      playerSensor = playerSensorGo.GetComponent<ISensor>();
    }

    float CaterpillarController.CaterpillarInput.getHorizontal() {
      return horizontalSpeed;
    }

    void Update() {
      if (camelflagIfNoFoundPlayer) {
        if (playerSensor != null && playerSensor.isStay()) {
          hasFoundPlayer = true;
          timeToCamelflagTimeInS = timeToCamelflagInS;
        }
        if (hasFoundPlayer) {
          // timer
          if (camelflagAgain) {
            timeToCamelflagTimeInS -= Time.deltaTime;
            if (timeToCamelflagTimeInS < 0.0f) {
              hasFoundPlayer = false;
              horizontalSpeed = 0.0f;
              return;
            }
          }

          // action
          if (isGrounded()) {
            horizontalSpeed = (canMoveForward() ? 1.0f : -1.0f) *
               Mathf.Sign(Common.Utils.getOrientation(gameObject));
          } else {
            float deltaX = Common.SceneConfig.getSceneConfig().GetPlayer().transform.position.x - transform.position.x;
            horizontalSpeed = Mathf.Sign(deltaX);
          }
        } else {
          horizontalSpeed = 0.0f;
        }
        return;
      }
      horizontalSpeed = (canMoveForward() ? 1.0f : -1.0f) *
         Mathf.Sign(Common.Utils.getOrientation(gameObject));
    }

    private bool canMoveForward() {
      return !isGroundDetected(frontSpaceDetector)
        && isGroundDetected(frontGroundDetector);
    }

    private bool isGrounded() {
      return isGroundDetected(belowGroundDetector);
    }

    private bool isGroundDetected(Rect rect) {
      return Common.Utils.isRectOverlap(rect, transform, groundLayerMask);
    }

    void OnDrawGizmosSelected() {
      Common.Utils.drawRectAsGizmos(frontSpaceDetector, isGroundDetected(frontSpaceDetector) ? Color.white : Color.blue, transform);
      Common.Utils.drawRectAsGizmos(frontGroundDetector, isGroundDetected(frontGroundDetector) ? Color.white : Color.red, transform);
      Common.Utils.drawRectAsGizmos(belowGroundDetector, isGroundDetected(belowGroundDetector) ? Color.white : Color.yellow, transform);
    }
  }
}
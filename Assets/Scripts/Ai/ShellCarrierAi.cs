using UnityEngine;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ai {
  public class ShellCarrierAi: MonoBehaviour, IInput {

    public Transform leftBound;
    public Transform rightBound;

    public float distanceThreshold = 0.2f;

    private float horizontal;

    enum MovingDirection {
      LEFT = 0,
      RIGHT,
    }
    private MovingDirection movingDirection = MovingDirection.LEFT;

    bool IInput.attack() {
      return false;
    }

    bool IInput.dash() {
      return false;
    }

    float IInput.getHorizontal() {
      return horizontal;
    }

    float IInput.getVertical() {
      return 0.0f;
    }

    bool IInput.interact() {
      return false;
    }

    bool IInput.jump() {
      return false;
    }

    bool IInput.meditation() {
      return false;
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
      if(movingDirection == MovingDirection.LEFT
        && Mathf.Abs(leftBound.position.x - transform.position.x) < distanceThreshold) {
        movingDirection = MovingDirection.RIGHT;
      }
      if(movingDirection == MovingDirection.RIGHT
        && Mathf.Abs(rightBound.position.x - transform.position.x) < distanceThreshold) {
        movingDirection = MovingDirection.LEFT;
      }
      if(movingDirection == MovingDirection.LEFT) {
        horizontal = -1.0f;
      } else {
        horizontal = 1.0f;
      }
    }

    public void resetStatus() {
      horizontal = 0.0f;
    }
  }
}


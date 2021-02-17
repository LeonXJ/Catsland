using UnityEngine;
using Panda;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;
using Catsland.Scripts.Controller.Blade;

namespace Catsland.Scripts.Ai.Blade {
  public class BladeAi : MonoBehaviour, IInput {

    private float horizon = 0f;
    private bool wantAttack = false;
    private bool wantJump = false;

    [Header("Detect")]
    public float visionDistance = 10f;
    public float visionAngle = 75f;
    public float senseDistance = 5f;

    // Once detect player, Whether to keep in detected status.
    public bool lockPlayer = true;
    private bool playerDetected = false;

    [Header("Attack")]
    public float attackDistance = 1f;

    [Header("Spin")]
    public float spinDistance = 5f;


    // References
    private BladeController controller;

    [Task]
    public void Idle() {
      horizon = 0f;
      wantAttack = false;
      wantJump = false;
      Task.current.Succeed();
    }

    [Task]
    public void DetectPlayer() {
      GameObject player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      if (player == null) {
        Task.current.Fail();
        return;
      }
      if (playerDetected) {
        Task.current.Succeed();
      }

      Vector2 delta = player.transform.position - transform.position;

      // Out of range
      if (delta.sqrMagnitude > visionDistance * visionDistance) {
        Task.current.Fail();
        return;
      }

      // Inside sense range
      if (delta.sqrMagnitude < senseDistance * senseDistance) {
        Task.current.Succeed();
        playerDetected = true;
        return;
      }

      Vector2 orientation = new Vector2(controller.GetOrientation(), 0f);
      // Out of angle range
      if (Vector2.Angle(orientation, delta) > visionAngle) {
        Task.current.Fail();
        return;
      }
      playerDetected = true;
      Task.current.Succeed();
    }

    [Task]
    public void MoveTowardsPlayer() {
      wantAttack = false;
      wantJump = false;
      GameObject player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      if (player == null) {
        Task.current.Fail();
        return;
      }
      float deltaX = player.transform.position.x - transform.position.x;
      horizon = Mathf.Sign(deltaX);
      Task.current.Succeed();
    }

    [Task]
    public void CanHitPlayer() {
      if (IsPlayerWithinDistance(attackDistance)) {
        Task.current.Succeed();
        return;
      }
      Task.current.Fail();
    }

    [Task]
    public void IsPlayerWithinSpinDistance() {
      if (IsPlayerWithinDistance(spinDistance)) {
        Task.current.Succeed();
        return;
      }
      Task.current.Fail();
    }

    private bool IsPlayerWithinDistance(float distance) {
      GameObject player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      if (player == null) {
        return false;
      }
      float orientation = controller.GetOrientation();
      Vector2 delta = player.transform.position - transform.position;

      return (delta.sqrMagnitude < distance *distance)
        && (orientation * delta.x > 0f);
    }

    [Task]
    public void Attack() {
      wantJump = false;
      wantAttack = true;

      if (controller.IsAttacking()) {
        Task.current.Succeed();
      }
    }

    // Spin, if true, spin forward otherwise backward.
    [Task]
    public void Spin(bool forward) {
      wantAttack = false;
      wantJump = true;

      GameObject player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      if (player == null) {
        Task.current.Fail();
        return;
      }
      float deltaX = player.transform.position.x - transform.position.x;
      horizon = (forward ? 1f : -1f) * Mathf.Sign(deltaX);

      if (controller.IsSpinning()) {
        Task.current.Succeed();
      }
    }

    public bool attack() {
      return wantAttack;
    }

    public bool dash() {
      throw new System.NotImplementedException();
    }

    public float getHorizontal() {
      return horizon;
    }

    public float getVertical() {
      throw new System.NotImplementedException();
    }

    public bool interact() {
      throw new System.NotImplementedException();
    }

    public bool jump() {
      return wantJump;
    }

    public bool jumpHigher() {
      throw new System.NotImplementedException();
    }

    public bool meditation() {
      throw new System.NotImplementedException();
    }

    public bool timeSlow() {
      throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start() {
      controller = GetComponent<BladeController>();
    }

    // Update is called once per frame
    void Update() {

    }
    private void OnDrawGizmos() {
      // Vision
      Common.Utils.DrawPieAsGizmos(
        visionDistance,
        Color.yellow,
        transform.position,
        new Vector2(Mathf.Sign(transform.lossyScale.x), 0f),
        visionAngle * 2f);

      // Sense
      Common.Utils.DrawCircleAsGizmos(
        senseDistance,
        Color.red,
        transform.position);
    }



  }
}

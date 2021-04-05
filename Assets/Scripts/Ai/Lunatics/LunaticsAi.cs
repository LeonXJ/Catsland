using UnityEngine;
using Panda;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;
using Catsland.Scripts.Controller.Lunatics;

namespace Catsland.Scripts.Ai.Lunatics {
  public class LunaticsAi : MonoBehaviour, IInput {

    private float horizon = 0f;
    private bool wantJump = false;
    private bool wantInteract = false;

    [Header("Detect")]
    public float senseDistance = 5f;
    public float lockTime = 3f;
    private float lastDetectTime = 0f;

    [Header("Jump")]
    public float jumpDistance = 5f;

    // Reference
    private LunaticsController controller;

    // Start is called before the first frame update
    void Start() {
      controller = GetComponent<LunaticsController>();
    }

    [Task]
    public void Idle() {
      horizon = 0f;
      wantJump = false;
      wantInteract = false;
      Task.current.Succeed();
    }

    [Task]
    public void DetectPlayer() {
      GameObject player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      if (player == null) {
        Task.current.Fail();
        return;
      }
      // lock
      if (Time.time - lastDetectTime < lockTime) {
        Task.current.Succeed();
        return;
      }

      Vector2 delta = player.transform.position - transform.position;

      // Inside sense range
      if (delta.sqrMagnitude < senseDistance * senseDistance) {
        Task.current.Succeed();
        lastDetectTime = Time.time;
        return;
      }
      Task.current.Fail();
    }

    [Task]
    public void GetAlarmIfNot() {
      if (controller.isInAlarm) {
        Task.current.Succeed();
        return;
      }
      wantInteract = true;
    }

    [Task]
    public void MoveTowardsPlayer() {
      wantJump = false;
      wantInteract = false;
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
    public void CanJumpHitPlayer() {
      GameObject player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      if (player == null) {
        Task.current.Fail();
        return;
      }
      float orientation = controller.GetOrientation();
      Vector2 delta = player.transform.position - transform.position;

      if (delta.sqrMagnitude > jumpDistance * jumpDistance
        || orientation * delta.x < 0f) {
        Task.current.Fail();
        return;
      }
      Task.current.Succeed();
    }

    [Task]
    public void Jump() {
      wantJump = true;
      wantInteract = false;
      Task.current.Succeed();
    }

    public void OnDamageByPass(DamageBypassInfo damageBypassInfo) {
      OnAttacked();
    }

    public void OnAttacked() {
      lastDetectTime = Time.time;
    }

    // Update is called once per frame
    void Update() {

    }
    public bool attack() {
      throw new System.NotImplementedException();
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
      return wantInteract;
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

  }
}

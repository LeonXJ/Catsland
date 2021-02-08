using System.Collections;
using System.Collections.Generic;
using Panda;
using UnityEngine;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;
using Catsland.Scripts.Controller.Shotgun;

namespace Catsland.Scripts.Ai.Shotgun {

  public class ShotgunAi : MonoBehaviour, IInput {

    private float horizon = 0f;
    private bool wantAttack = false;

    [Header("Detect")]
    public float visionDistance = 10f;
    public float visionAngle = 75f;
    public float senseDistance = 5f;

    [Header("Shoot")]
    public float openFireDistance = 5f;

    // Reference
    private ShotgunController controller;

    public bool attack() {
      return wantAttack;
    }

    public bool dash() {
      throw new System.NotImplementedException();
    }

    [Task]
    public void Idle() {
      wantAttack = false;
      Task.current.Succeed();
    }

    [Task]
    public void DetectPlayer() {
      GameObject player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      if (player == null) {
        Task.current.Fail();
        return;
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
        return;
      }

      Vector2 orientation = new Vector2(controller.GetOrientation(), 0f);
      // Out of angle range
      if (Vector2.Angle(orientation, delta) > visionAngle) {
        Task.current.Fail();
        return;
      }
      Task.current.Succeed();
    }
    
    [Task]
    public void MoveTowardsPlayer() {
      wantAttack = false;
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
    public void Shoot() {
      wantAttack = true;
      Task.current.Succeed();
    }

    [Task]
    public void CanHitPlayer() {
      GameObject player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      if (player == null) {
        Task.current.Fail();
        return;
      }
      float orientation = controller.GetOrientation();
      Vector2 delta = player.transform.position - transform.position;

      if (delta.sqrMagnitude > openFireDistance * openFireDistance
        || orientation * delta.x < 0f) {
        Task.current.Fail();
        return;
      }
      Task.current.Succeed();
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
      throw new System.NotImplementedException();
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
      controller = GetComponent<ShotgunController>();
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

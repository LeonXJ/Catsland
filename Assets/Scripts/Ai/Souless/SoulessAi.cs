using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;
using Catsland.Scripts.Controller.Souless;

namespace Catsland.Scripts.Ai.Souless {
  public class SoulessAi : MonoBehaviour, IInput {

    [Header("Detect")]
    public float senseDistance = 10f;

    private float horizon = 0f;
    private bool wantAttack = false;

    // Reference
    private SoulessController controller;

    // Start is called before the first frame update
    void Start() {
      controller = GetComponent<SoulessController>();
    }

    // Update is called once per frame
    void Update() {

    }

    [Task]
    public void Idle() {
      horizon = 0f;
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
    
      // Inside sense range
      if (delta.sqrMagnitude < senseDistance * senseDistance) {
        Task.current.Succeed();
        return;
      }
      Task.current.Fail();
    }

    [Task]
    public void RiseIfLying() {
      if (controller.isLaydown) {
        controller.isLaydown = false;
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
    public void Cast() {
      wantAttack = true;
      Task.current.Succeed();
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

  }
}

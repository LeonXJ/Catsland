using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ai.ShieldMan {
  public class ShieldManAi : MonoBehaviour, IInput {

    public GameObject playerSensorGo;

    private bool wantAttack;
    private Vector2 v;

    private ISensor playerSensor;

    public bool attack() => wantAttack;

    public bool dash() {
      throw new System.NotImplementedException();
    }

    public float getHorizontal() => v.x;

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
      playerSensor = playerSensorGo.GetComponent<ISensor>();
    }

    // Update is called once per frame
    void Update() {

    }

    [Task]
    public void moveTowardsPlayer() {
      resetStatus();
      Vector3 delta = SceneConfig.getSceneConfig().GetPlayer().transform.position - transform.position;
      v = new Vector2(delta.x, 0f);
      Task.current.Succeed();
    }

    [Task]
    public void isPlayerInAttackRange() {
      if(playerSensor.isStay()) {
        Task.current.Succeed();
      } else {
        Task.current.Fail();
      }
    }

    [Task]
    public void doAttack() {
      resetStatus();
      wantAttack = true;
      Task.current.Succeed();
    }

    public void resetStatus() {
      v = Vector2.zero;
      wantAttack = false;
    }
  }
}

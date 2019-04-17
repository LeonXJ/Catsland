using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ai {
  public class BeeAi: MonoBehaviour, BeeController.BeeInput {

    public Transform[] patrolPoints;
    public float distanceThreshold = 0.2f;

    public GameObject playerSensorGo;
    public bool lockOn = false;

    private bool wantAttack;
    private Vector2 v;

    private int currentPatrolPoint = 0;
    private ISensor playerSensor;
    private BeeController controller;

    void Awake() {
      playerSensor = playerSensorGo.GetComponent<ISensor>();
      controller = GetComponent<BeeController>();
    }

    void Start() {
      TriggerBasedSensor sensor = playerSensor as TriggerBasedSensor;
      sensor.whitelistGos.Add(Common.SceneConfig.getSceneConfig().GetPlayer());
    }

    public bool attack() {
      return wantAttack;
    }

    public float getHorizontal() {
      return v.x;
    }

    public float getVertical() {
      return v.y;
    }

    [Task]
    public void isLockOn() {
      if(lockOn) {
        Task.current.Succeed();
      } else {
        Task.current.Fail();
      }
    }

    [Task]
    public void isPlayerInReactDistance() {
      if(playerSensor.isStay()) {
        Task.current.Succeed();
      } else {
        Task.current.Fail();
      }
    }

    [Task]
    public void patrol() {
      resetStatus();
      if(currentPatrolPoint >= patrolPoints.Length) {
        v = Vector2.zero;
        Task.current.Succeed();
        return;
      }
      Vector2 target = patrolPoints[currentPatrolPoint].position;
      Vector2 delta = target - new Vector2(transform.position.x, transform.position.y);
      if(delta.sqrMagnitude < distanceThreshold * distanceThreshold) {
        currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
      } else {
        v = delta;
      }
      Task.current.Succeed();
    }

    [Task]
    public void doCharge() {
      resetStatus();
      Utils.succeedOnConditionElseDo(controller.consumeHasCharged(),
        () => {
          wantAttack = true;
        });
    }

    public void resetStatus() {
      v = Vector2.zero;
      wantAttack = false;
    }
  }
}

using System.Collections;
using UnityEngine;
using Panda;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ai {
  public class Bandit :MonoBehaviour, IInput {
    public GameObject arrowSensorGo;
    public GameObject chopSensorGo;

    private BanditController controller;
    private ISensor arrowSensor;
    private ISensor chopSensor;
    private GameObject playerGo;

    private bool wantAttack = false;
    private float wantHorizontal = 0.0f;

    void Awake() {
      controller = GetComponent<BanditController>();
      playerGo = GameObject.FindGameObjectWithTag("Player");
      arrowSensor = arrowSensorGo.GetComponent<ISensor>();
      chopSensor = chopSensorGo.GetComponent<ISensor>();
    }

    public bool attack() {
      return wantAttack;
    }

    public float getHorizontal() {
      return wantHorizontal;
    }

    public float getVertical() {
      return 0.0f;
    }

    public bool jump() {
      return false;
    }

    [Task]
    public void isIncomingArrow() {
      if(arrowSensor.isStay()) {
        Task.current.Succeed();
      } else {
        Task.current.Fail();
      }
    }

    [Task]
    public void isPlayerInChopRange() {
      if(chopSensor.isStay()) {
        Task.current.Succeed();
      } else {
        Task.current.Fail();
      }
    }

    [Task]
    public void moveTowardsPlayer() {
      resetControlStatus();
      Vector2 delta = playerGo.transform.position - transform.position;
      wantHorizontal = Mathf.Sign(delta.x);
      Task.current.Succeed();
    }

    [Task]
    public void isPlayerInFront() {
      Vector2 delta = playerGo.transform.position - transform.position;
      float orientation = controller.getOrientation();
      if(delta.x * orientation > 0.0f) {
        Task.current.Succeed();
      } else {
        Task.current.Fail();
      }
    }

    [Task]
    public void chop() {
      resetControlStatus();
      wantAttack = true;
    }

    [Task]
    public void stop() {
      resetControlStatus();
      Task.current.Succeed();
    }

    private IEnumerator wait(float time) {
      yield return new WaitForSeconds(time);
    }

    private void resetControlStatus() {
      wantHorizontal = 0.0f;
      wantAttack = false;
    }
  }
}

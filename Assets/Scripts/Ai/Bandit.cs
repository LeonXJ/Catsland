using System.Collections;
using UnityEngine;
using Panda;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ai {
  public class Bandit :MonoBehaviour, IInput {
    public GameObject arrowSensorGo;
    public GameObject chopSensorGo;
    public GameObject viewSensorGo;
    public Transform standPoint;
    public float lockPlayerForSeconds = 3.0f;
    public float reachTargetDistance = 0.1f;

    private BanditController controller;
    private ISensor arrowSensor;
    private ISensor chopSensor;
    private ISensor viewSensor;
    private GameObject playerGo;

    private bool wantAttack = false;
    private float wantHorizontal = 0.0f;
    private float playerImageFreshness = -10.0f;

    void Awake() {
      controller = GetComponent<BanditController>();
      playerGo = GameObject.FindGameObjectWithTag("Player");
      arrowSensor = arrowSensorGo.GetComponent<ISensor>();
      chopSensor = chopSensorGo.GetComponent<ISensor>();
      viewSensor = viewSensorGo.GetComponent<ISensor>();
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

    public bool dash() {
      return false;
    }

    public bool meditation() {
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
      moveTowardsPoint(playerGo.transform.position);
      Task.current.Succeed();
    }

    [Task]
    public void moveTowardsStandPoint() {
      moveTowardsPoint(standPoint.position);
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

    [Task]
    public void knowPlayerPosition() {
      if(playerImageFreshness > 0.0f) {
        Task.current.Succeed();
      } else {
        Task.current.Fail();
      }
    }

    void Update() {
      if(viewSensor.isStay()) {
        playerImageFreshness = lockPlayerForSeconds;
      } else if(playerImageFreshness > 0.0f) {
        playerImageFreshness -= Time.deltaTime;
      }
    }

    public void damage(DamageInfo damageInfo) {
      playerImageFreshness = lockPlayerForSeconds;
    }

    private void moveTowardsPoint(Vector3 targetPoint) {
      resetControlStatus();
      Vector2 delta = targetPoint - transform.position;
      if(Mathf.Abs(delta.x) > reachTargetDistance) {
        wantHorizontal = Mathf.Sign(delta.x);
      }
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

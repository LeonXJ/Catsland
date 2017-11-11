using System.Collections;
using UnityEngine;
using Panda;

using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ai {
  [RequireComponent(typeof(CharacterController))]
  public class DoNothing :MonoBehaviour, IInput {

    enum CharacterAction {
      Attack,
      Jump,
    }

    public float attackDistance = 5.0f;
    public float attackVerticalRange = 0.5f;
    public Transform[] petrolPoints;
    public float pointReachRange = 0.2f;

    private bool wantAttack = false;
    private float wantHorizontal = 0.0f;

    private PlayerController controller;
    private GameObject playerGo;
    private int currentTargetPetrolPoint = -1;

    void Awake() {
      controller = GetComponent<PlayerController>();
      playerGo = GameObject.FindGameObjectWithTag("Player");
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
    public void isPlayerInRange() {
      if(playerGo == null) {
        Task.current.Fail();
        return;
      }

      Vector2 delta = playerGo.transform.position - transform.position;

      // check vertical
      if (Mathf.Abs(delta.y) > attackVerticalRange) {
        Task.current.Fail();
        return;
      }

      // check horizontal distance 
      if(Mathf.Abs(delta.x) > attackDistance) {
        Task.current.Fail();
        return;
      }

      // check orientation
      if (delta.x * controller.getOrientation() < 0.0f) {
        Task.current.Fail();
        return;
      }

      Task.current.Succeed();
    }

    [Task]
    public void doDrawing(float targetDrawIntensity) {
      resetControllStatus();

      wantAttack = true;
      if(controller.getDrawIntensity() > targetDrawIntensity) {
        Task.current.Succeed();
      }
    }

    [Task]
    public void doShoot() {
      resetControllStatus();

      wantAttack = false;
      Task.current.Succeed();
    }

    [Task]
    public void isMovingToPetrolPoint() {

      if(petrolPoints == null || petrolPoints.Length == 0 || currentTargetPetrolPoint < 0) {
        Task.current.Fail();
        return;
      }

      float delta = petrolPoints[currentTargetPetrolPoint].position.x - transform.position.x;

      if(Mathf.Abs(delta) < pointReachRange) {
        Task.current.Fail();
      } else {
        Task.current.Succeed();
      }

    }

    [Task]
    public void setPetrolPoint() {
      if(petrolPoints == null || petrolPoints.Length == 0) {
        Task.current.Fail();
        return;
      }
      if(currentTargetPetrolPoint < 0) {
        currentTargetPetrolPoint = 0;
      } else {
        currentTargetPetrolPoint = (currentTargetPetrolPoint + 1) % petrolPoints.Length;
      }
      Task.current.Succeed();
    }

    [Task]
    public void moveToPetrolPoint() {
      resetControllStatus();

      if(petrolPoints == null || petrolPoints.Length == 0 || currentTargetPetrolPoint < 0) {
        Task.current.Fail();
        return;
      }

      float delta = petrolPoints[currentTargetPetrolPoint].position.x - transform.position.x;

      // move to target point
      delta = petrolPoints[currentTargetPetrolPoint].position.x - transform.position.x;
      if(delta > 0.0f) {
        wantHorizontal = 1.0f;
      } else {
        wantHorizontal = -1.0f;
      }
      Task.current.Succeed();
    }

    [Task]
    public void stop() {
      resetControllStatus();
      Task.current.Succeed();
    }

    private void resetControllStatus() {
      wantHorizontal = 0.0f;
      wantAttack = false;
    }
  }
}

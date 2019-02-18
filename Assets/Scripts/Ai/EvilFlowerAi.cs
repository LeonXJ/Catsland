using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using Catsland.Scripts.Controller;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Ai {
  [RequireComponent(typeof(EvilFlowerController))]

  public class EvilFlowerAi: MonoBehaviour, EvilFlowerController.EvilFlowerInput {

    public float detectRange = 5.0f;

    private EvilFlowerController controller;
    private Utils utils;

    private bool wantAttack;

    void Awake() {
      controller = GetComponent<EvilFlowerController>();
      utils = new Utils(gameObject);
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public bool attack() {
      bool preWantAttack = wantAttack;
      wantAttack = false;
      return preWantAttack;
    }

    [Task]
    public void isPlayerInReactDistance() {
      utils.setTaskSucceedIfPlayerInDistanceOrFail(detectRange);
    }

    [Task]
    public void doShoot() {
      resetStatus();
      wantAttack = true;
      Task.current.Succeed();
    }

    [Task]
    public void doNothing() {
      resetStatus();
      Task.current.Succeed();
    }

    public void resetStatus() {
      wantAttack = false;
    }
  }
}

using UnityEngine;
using Panda;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ai {
  public class RoyalGuardAi: MonoBehaviour, RoyalGuardController.RoyalGuardInput {

    public Vector2 reactDistance = new Vector2(5.0f, 2.0f);
    private Utils utils;

    private RoyalGuardController controller;

    private float horizontal;
    private bool wantChop;
    private bool wantJumpSmash;
    private bool wantSummon;

    public void Awake() {
      controller = GetComponent<RoyalGuardController>();
      utils = new Utils(gameObject);
    }

    [Task]
    public void isPlayerInReactDistance() {
      utils.setTaskSucceedIfPlayerInDistanceOrFail(reactDistance);
    }

    [Task]
    public void faceToPlayer() {
      resetStatus();
      float horizontalDelta = utils.getDistanceToPlayer().x;
      Utils.succeedOnConditionElseDo(controller.getOrientation() * horizontalDelta > 0.0f,
        () => {
          horizontal = Mathf.Sign(horizontalDelta);
        });
    }

    [Task]
    public void comboChop() {
      resetStatus();
      Utils.succeedOnConditionElseDo(controller.getChopCombo() > 4,
        () => {
          wantChop = true;
        });
    }

    [Task]
    public void doSummon() {
      resetStatus();
      Utils.succeedOnConditionElseDo(controller.consumeHasAoeUnleashed(),
        () => {
          wantSummon = true;
        });
    }

    [Task]
    public void doJumpSmash() {
      resetStatus();
      Utils.succeedOnConditionElseDo(controller.consumeHasJumpSmashUnleashed(),
        () => {
          wantJumpSmash = true;
        });
    }

    public void resetStatus() {
      horizontal = 0.0f;
      wantChop = false;
      wantJumpSmash = false;
      wantSummon = false;
    }

    public float getHorizontal() {
      return horizontal;
    }

    public bool chop() {
      return wantChop;
    }

    public bool jumpSmash() {
      return wantJumpSmash;
    }

    public bool summon() {
      return wantSummon;
    }
  }
}

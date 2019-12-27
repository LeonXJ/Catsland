using UnityEngine;
using Panda;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ai {
  public class BanditHead: MonoBehaviour, HeadOfBanditController.HeadOfBanditInput {

    // The function to perform an action.
    delegate void PerformAction();

    public bool activated = false;
    public bool willDisplay = true;

    public float knifeReachDistance = 10.0f;
    public float chargeReachDistance = 6.0f;
    public float jumpSmashReachDistance = 3.0f;
    public float verticalReachDistance = 5.0f;

    private float horizontal;
    private bool wantJumpSmash;
    private bool wantSpell;
    private bool wantCharge;
    private bool wantDisplay;
    private bool hasDisplayed = false;

    private bool taskStarted = false;

    private GameObject playerGoCache;
    private GameObject playerGo {
      get {
        if(playerGoCache == null) {
          playerGoCache = GameObject.FindGameObjectWithTag(Tags.PLAYER);
        }
        return playerGoCache;
      }
    }
    private HeadOfBanditController controller;

    public bool charge() {
      return wantCharge;
    }

    public float getHorizontal() {
      return horizontal;
    }

    public bool jumpSmash() {
      return wantJumpSmash;
    }

    public bool spell() {
      return wantSpell;
    }

    private void resetStatus() {
      horizontal = 0.0f;
      wantJumpSmash = false;
      wantSpell = false;
      wantCharge = false;
      wantDisplay = false;
    }

    void Start() {
      controller = GetComponent<HeadOfBanditController>();
    }

    void Update() {
      if(playerGo == null) {
      }
    }

    [Task]
    public void holdIfNotActivate() {
      resetStatus();
      if (activated) {
        Task.current.Succeed();
      } else {
        Task.current.Fail();
      }
    }

    [Task]
    public void isPlayerInKnifeReachDistance() {
      setTaskSucceedElseFailIfInDistance(knifeReachDistance);
    }

    [Task]
    public void shouldDisplay() {
      if (!willDisplay) {
        Task.current.Fail();
      }
      if (!hasDisplayed) {
        Task.current.Succeed();
      } else {
        Task.current.Fail();
      }
    }

    [Task]
    public void doDisplay() {
      resetStatus();
      wantDisplay = true;
      hasDisplayed = true;
      performAndWaitDoneAction(
        controller.canDisplay(), () => {
          wantDisplay = true;
        },
        controller.status == HeadOfBanditController.Status.DISPLAY_DONE);
    }

    [Task]
    public void throwKnife() {
      performAndWaitDoneAction(
        controller.canSpell(), () => {
          wantSpell = true;
        },
        controller.status == HeadOfBanditController.Status.SPELL_REST);
    }

    [Task]
    public void performCharge() {
      performAndWaitDoneAction(
        controller.canCharge(), () => {
          wantCharge = true;
        },
        controller.status == HeadOfBanditController.Status.CHARGE_REST);
    }

    [Task]
    public void performJumpSmash() {
      performAndWaitDoneAction(
        controller.canJumpSmash(), () => {
          wantJumpSmash = true;
        },
        controller.status == HeadOfBanditController.Status.JUMP_SMASH_REST);
    }

    [Task]
    public void isPlayerInChargeReachDistance() {
      setTaskSucceedElseFailIfInDistance(chargeReachDistance);
    }

    [Task]
    public void isPlayerInJumpSmashReachDistance() {
      setTaskSucceedElseFailIfInDistance(jumpSmashReachDistance);
    }

    [Task]
    public void faceToPlayer() {
      resetStatus();
      float horizontalDelta = getDistanceToPlayer().x;
      // Keep checking the orientation until successfully set orientation.
      if(controller.getOrientation() * horizontalDelta > 0.0f) {
        Task.current.Succeed();
        return;
      }
      if(controller.canAdjustOrientation()) {
        horizontal = Mathf.Sign(horizontalDelta);
      }
    }

    private void setTaskSucceedElseFailIfInDistance(float distance) {
      Vector2 delta = getDistanceToPlayer();
      if(Mathf.Abs(delta.x) < distance && Mathf.Abs(delta.y) < verticalReachDistance) {
        Task.current.Succeed();
      } else {
        Task.current.Fail();
      }
    }

    private Vector2 getDistanceToPlayer() {
      return playerGo.transform.position - gameObject.transform.position;
    }

    private void performAndWaitDoneAction(
      bool canPerformAction,
      PerformAction performAction,
      bool hasActionDone) {
      resetStatus();
      if(!taskStarted) {
        if(canPerformAction) {
          taskStarted = true;
          performAction();
        }
        // otherwise waiting for opportunity to perform.
        return;
      }
      // taskStarted, check whether it has finished.
      if(hasActionDone) {
        taskStarted = false;
        Task.current.Succeed();
        return;
      }
    }

    public bool display() {
      return wantDisplay;
    }

    public void activate() {
      activated = true;
    }
  }
}

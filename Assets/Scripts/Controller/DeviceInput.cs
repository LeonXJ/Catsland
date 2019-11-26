using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class DeviceInput: MonoBehaviour, IInput,
    RoyalGuardController.RoyalGuardInput,
    EvilFlowerController.EvilFlowerInput,
    BeeController.BeeInput {

    private bool wantAttack = false;
    private float horizontal = 0f;
    private float vertical = 0f;
    private bool wantJump = false;
    private bool wantJumpHigher = false;
    private bool wantDash = false;
    private bool lastWantDash = false;
    private bool wantInteract = false;

    void Update() {
      // Because controller trigger is axis, we simulate snap button here.
      wantDash = false;
      if(Input.GetButton("Dash")) {
        if(!lastWantDash) {
          wantDash = true;
        }
        lastWantDash = true;
      } else {
        lastWantDash = false;
      }

      horizontal = Input.GetAxis("Horizontal");
      vertical = Input.GetAxis("Vertical");
      wantAttack = Input.GetButton("Attack");
      wantJump = Input.GetButtonDown("Jump");
      wantJumpHigher = Input.GetButton("Jump");
      wantInteract = Input.GetButton("Interact");
    }

    void OnDisable() {
      resetInput();
    }

    public void resetInput() {
      horizontal = 0f;
      vertical = 0f;
      wantAttack = false;
      wantJump = false;
      wantJumpHigher = false;
      wantDash = false;
      wantInteract = false;
    }

    public bool attack() {
      return wantAttack;
    }

    public float getHorizontal() {
      return horizontal;
    }

    public float getVertical() {
      return vertical;
    }

    public bool jump() {
      return wantJump;
    }

    public bool jumpHigher() {
      return wantJumpHigher;
    }

    public bool dash() {
      return wantDash;
    }

    public bool meditation() {
      return Input.GetButton("Interact");
    }

    public bool charge() {
      return Input.GetButtonDown("Charge");
    }

    public bool jumpSmash() {
      return Input.GetButtonDown("Jump");
    }

    public bool spell() {
      return Input.GetButtonDown("Dash");
    }

    public bool interact() {
      return wantInteract;
    }

    public bool chop() {
      return Input.GetButtonDown("Attack");
    }

    public bool summon() {
      return Input.GetButton("Interact");
    }
  }
}

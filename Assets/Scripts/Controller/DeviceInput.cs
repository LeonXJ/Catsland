using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class DeviceInput: MonoBehaviour, IInput, HeadOfBanditController.HeadOfBanditInput {

    public float dashAxisDeadzone = 0.2f;

    private bool lastWantDash = false;
    private bool curWantDash = false;

    void Update() {
      // Because controller trigger is axis, we simulate snap button here.
      curWantDash = false;
      if(Input.GetAxis("Dash") > dashAxisDeadzone) {
        if(!lastWantDash) {
          curWantDash = true;
        }
        lastWantDash = true;
      } else {
        lastWantDash = false;
      }

    }

    public bool attack() {
      return Input.GetButton("Attack");
    }

    public float getHorizontal() {
      return Input.GetAxis("Horizontal");
    }

    public float getVertical() {
      return Input.GetAxis("Vertical");
    }

    public bool jump() {
      return Input.GetButtonDown("Jump");
    }

    public bool dash() {
      return Input.GetButtonDown("Dash") || curWantDash;
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
      return Input.GetButtonDown("Interact");
    }
  }
}

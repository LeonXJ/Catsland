using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class DeviceInput: MonoBehaviour, IInput,
    RoyalGuardController.RoyalGuardInput,
    EvilFlowerController.EvilFlowerInput,
    BeeController.BeeInput {

    private InputMaster inputMaster;

    void Awake() {
      inputMaster = new InputMaster();
    }

    public bool attack() {
      return inputMaster.General.Shoot.ReadValue<float>() > Mathf.Epsilon;
    }

    public bool timeSlow() {
      return inputMaster.General.Focus.ReadValue<float>() > Mathf.Epsilon;
    }

    public float getHorizontal() {
      return inputMaster.General.Move.ReadValue<Vector2>().x;
    }

    public float getVertical() {
      return inputMaster.General.Move.ReadValue<Vector2>().y;
    }

    public bool jump() {
      return inputMaster.General.Jump.triggered;
    }

    public bool jumpHigher() {
      return inputMaster.General.JumpHigher.ReadValue<float>() > Mathf.Epsilon;
    }

    public bool dash() {
      return inputMaster.General.Dash.triggered;
    }

    public bool meditation() {
      return false;
      //return Input.GetButton("Interact");
    }

    public bool charge() {
      return false;
      // return Input.GetButtonDown("Charge");
    }

    public bool jumpSmash() {
      return false;
      // return Input.GetButtonDown("Jump");
    }

    public bool spell() {
      return false;
      // return Input.GetButtonDown("Dash");
    }

    public bool interact() {
      return inputMaster.General.Interact.triggered;
    }

    public bool chop() {
      return false;
      //return Input.GetButtonDown("Attack");
    }

    public bool summon() {
      return false;
      //return Input.GetButton("Interact");
    }

    private void OnEnable() {
      inputMaster.General.Enable();
    }

    private void OnDisable() {
      inputMaster.General.Disable();
    }
  }
}

using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class DeviceInput: MonoBehaviour, IInput, HeadOfBanditController.HeadOfBanditInput {

    public bool attack() {
      return Input.GetButton("Fire1");
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
      return Input.GetButtonDown("Dash");
    }

    public bool meditation() {
      return Input.GetButton("Meditation");
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

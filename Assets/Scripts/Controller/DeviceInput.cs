using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class DeviceInput :MonoBehaviour, IInput {

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
  }
}

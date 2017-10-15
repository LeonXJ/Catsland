using UnityEngine;

namespace Catsland.Scripts.CharacterController {
  public class DeviceInput :MonoBehaviour, IInput {

  public bool attack() {
    return Input.GetButton("Fire1");
  }

  public float getHorizontal() {
    return Input.GetAxis("Horizontal");
  }

  public bool jump() {
    return Input.GetButtonDown("Jump");
  }
  }
}

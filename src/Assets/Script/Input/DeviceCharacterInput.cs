using UnityEngine;

namespace Catslandx.Script.Input {

  /** The device (keyboard/mouse/joy) input. */
  public class DeviceCharacterInput : AbstractCharacterInput {

    public override void updateInput(float deltaTime) {
      wantDirection = new Vector2(
        UnityEngine.Input.GetAxis("Horizontal"), UnityEngine.Input.GetAxis ("Vertical"));
      // TODO: do not use specific keycode, use key mapping instead.
      wantJump = UnityEngine.Input.GetKeyDown(KeyCode.J);
      wantDash = UnityEngine.Input.GetKeyDown(KeyCode.K);
    }
  }
}

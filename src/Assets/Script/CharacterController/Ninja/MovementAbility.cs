using Catslandx.Script.CharacterController.Common;
using UnityEngine;

namespace Catslandx.Script.CharacterController.Ninja {

  /** The ablitiy to move. */
  public class MovementAbility : AbstractCharacterAbility {

    public float maxRunSpeed;
    public float maxCrouchSpeed;
    public float jumpInitialSpeed;
    public float crouchControlSensitive = - Mathf.Epsilon;

    private bool isCrouch;

    public void setIsCrouch(bool isCrouch) {
      this.isCrouch = isCrouch;
    }

    public bool getIsCrouch() {
      return isCrouch;
    }

    public float getCurrentMaxSpeed() {
      return isCrouch ? maxCrouchSpeed : maxRunSpeed;
    }

  }
}

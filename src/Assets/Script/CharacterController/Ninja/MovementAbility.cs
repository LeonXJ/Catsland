using Catslandx.Script.CharacterController.Common;
using UnityEngine;

namespace Catslandx.Script.CharacterController.Ninja {

  /** The ablitiy to move. */
  public class MovementAbility : AbstractCharacterAbility {
    public enum Orientation {
      Left = 0,
      Right = 1,
    }

    public float maxRunSpeed;
    public float maxCrouchSpeed;
    public float jumpInitialSpeed;
    public float crouchControlSensitive = - Mathf.Epsilon;

    private Orientation orientation;
    private bool isCrouch;

    public void setOrientation(Orientation orientation) {
      this.orientation = orientation;
    }

    public Orientation getOrientation() {
      return orientation;
    }

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

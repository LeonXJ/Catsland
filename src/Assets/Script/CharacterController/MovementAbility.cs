using UnityEngine;
using System.Collections;

namespace Catslandx {
  public class MovementAbility : AbstractCharacterAbility {

    public enum Orientation {
      Left = 0,
      Right = 1,
    }

    private Orientation orientaton;

    public float maxRunSpeed;
    public float maxCrouchSpeed;
    public float jumpInitialSpeed;
    public float crouchControlSensitive = 0.1f;
  }
}

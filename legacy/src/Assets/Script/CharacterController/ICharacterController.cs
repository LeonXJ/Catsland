using UnityEngine;
using System;

namespace Catslandx.Script.CharacterController {

  /** The interface of character controller. */
  public interface ICharacterController {

    // Transits to a status.
    IStatus transitToStatus<T>() where T : IStatus;

    // TODO: how is it used ????
    IStatus innerTransteToState();

    // Returns the current status.
    IStatus getCurrentState();

    // Reset to initial.
    IStatus reset();

    void setOrientation(Orientation orientation);

    Orientation getOrientation();

    Vector2 transformRightOrientationVectorToCurrentOrientation(Vector2 rightOrientationVector);
  }
}

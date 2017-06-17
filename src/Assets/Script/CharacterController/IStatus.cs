using Catslandx.Script.Input;
using Catslandx.Script.Sensor;
using System.Collections.Generic;

namespace Catslandx.Script.CharacterController {

  /** The status of the character. */
  public interface IStatus {

    /** Performs on enter this status. */
    void onEnter(IStatus previousStatus);

    /** Performs on staying in this status. */
    IStatus update(
        Dictionary<SensorEnum, ISensor> sensors,
        ICharacterInput input,
        float deltaTime);

    /** Performs on exit this status. */
    void onExit(IStatus nextStatus);
  }
}

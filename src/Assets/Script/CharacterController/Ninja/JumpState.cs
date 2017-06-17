using UnityEngine;
using System.Collections.Generic;
using Catslandx.Script.CharacterController.Common;
using Catslandx.Script.Sensor;
using Catslandx.Script.Input;

namespace Catslandx.Script.CharacterController.Ninja {

  /** Jump state for character controller.
   * 
   * Required:
   * - MovementAbility
   * - RigidBody2D
   */
  public class JumpState : AbstractStatus {

    private MovementAbility movementAbility;
    private Rigidbody2D rigidBody;

    public JumpState(GameObject gameObject, StatusFactory stateFactory)
        : base(gameObject, stateFactory) {
      movementAbility = getComponent<MovementAbility>();
      rigidBody = getComponent<Rigidbody2D>();
    }

    public override IStatus update(
        Dictionary<SensorEnum, ISensor> sensors,
        ICharacterInput input,
        float deltaTime) {
      // check ground sensor
      ISensor groundSensor;
      if (sensors.TryGetValue(SensorEnum.ON_GROUND_SENSOR, out groundSensor)
          && groundSensor.isInTrigger()) {
        // on ground
        return getStateFactory().getState<MovementStatus>();
      }
      // check relay
      float velticalSpeed = rigidBody.velocity.y;
      ISensor relaySensor;
      if (sensors.TryGetValue(SensorEnum.RELAY_SENSOR, out relaySensor)
          && relaySensor.isInTrigger()) {
        if (input.wantDash()) {
          // return dash status
        }
        if (input.wantJump()) {
          velticalSpeed = movementAbility.jumpInitialSpeed;
        }
      }
      // horizontal speed
      float horizontalSpeed = input.wantDirection().x * movementAbility.maxRunSpeed;
      rigidBody.velocity = new Vector2(horizontalSpeed, velticalSpeed);
      return this;
    }
  }
}

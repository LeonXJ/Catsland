using UnityEngine;
using System.Collections.Generic;
using Catslandx.Script.CharacterController.Common;
using Catslandx.Script.Sensor;
using Catslandx.Script.Input;

namespace Catslandx.Script.CharacterController.Ninja {

  /** Movement state for character controller.
   *
   * Require:
   * - MovementAbility
   * - Rigidbody2D
   */ 
  public class MovementStatus :AbstractStatus {

    private MovementAbility movementAbility;
    private Rigidbody2D rigidBody;

    public MovementStatus(GameObject gameObject, StatusFactory stateFactory)
        : base(gameObject, stateFactory) {
      movementAbility = getComponent<MovementAbility>();
      rigidBody = getComponent<Rigidbody2D>();
    }

    public override IStatus update(
        Dictionary<SensorEnum, ISensor> sensors,
        ICharacterInput input,
        float deltaTime) {
      // orientation
      if(Mathf.Abs(input.wantDirection().x) > Mathf.Epsilon) {
        characterController.setOrientation(getOrientation(input.wantDirection().x));
      }
      // check ground sensor
      ISensor groundSensor = sensors[SensorEnum.ON_GROUND_SENSOR];
      if (groundSensor == null || !groundSensor.isInTrigger()) {
        return getStateFactory().getState<JumpState>();        
      }
      // horizontal speed: crouch or run 
      // check ceiling
      ISensor ceilingSensor = sensors[SensorEnum.CEILING_SENSOR];
      if((ceilingSensor != null && ceilingSensor.isInTrigger())
        || input.wantDirection().y < movementAbility.crouchControlSensitive) {
        // something on the ceiling, need to crouch
        movementAbility.setIsCrouch(true);
      } else {
        movementAbility.setIsCrouch(false);
      }
      float horizontalSpeed =
        input.wantDirection().x * movementAbility.getCurrentMaxSpeed();
      // vertical speed: jump
      if(input.wantJump()) {
        // jump
        rigidBody.velocity = new Vector2(horizontalSpeed, movementAbility.jumpInitialSpeed);
        return getStateFactory().getState<JumpState>();
      } else if(input.wantAttack()) {
        IStatus meleeStatus = getStateFactory().getState<MeleeStatus>();
        if(meleeStatus.isEligible()) {
          return meleeStatus;
        }
      }
      rigidBody.velocity = new Vector2(horizontalSpeed, rigidBody.velocity.y);
      return this;
    }

    private Orientation getOrientation(float horizontalSpeed) {
      return horizontalSpeed > 0.0f
          ? Orientation.Right
          : Orientation.Left;
    }
  }
}

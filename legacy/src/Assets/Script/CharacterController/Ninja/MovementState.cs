using UnityEngine;
using System.Collections.Generic;
using Catslandx.Script.CharacterController.Common;
using Catslandx.Script.Common;
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
	  if(groundSensor == null || !groundSensor.isInTrigger()) {
		return getStateFactory().getState<JumpState>();
	  }
	  // horizontal speed: crouch or run 
	  // check ceiling
	  bool isCrouch = false;
	  ISensor ceilingSensor = getSensorOrNull(sensors, SensorEnum.CEILING_SENSOR);
	  if((ceilingSensor != null && ceilingSensor.isInTrigger())
		|| input.wantDirection().y < movementAbility.crouchControlSensitive) {
		// something on the ceiling, need to crouch
		movementAbility.setIsCrouch(true);
		isCrouch = true;
	  } else {
		movementAbility.setIsCrouch(false);
		isCrouch = false;
	  }

	  // check left and right sensor and decide horizontal speed
	  ISensor rearSensor = getSensorOrNull(sensors, SensorEnum.REAR_SENSOR);
	  ISensor frontSensor = getSensorOrNull(sensors, SensorEnum.FRONT_SENSOR);
	  float horizontalSpeed =
		CharacterHelper.getHorizontalSpeed(input.wantDirection().x,
		characterController.getOrientation(),
		frontSensor != null ? frontSensor.isInTrigger() : false,
		rearSensor != null ? rearSensor.isInTrigger() : false,
		movementAbility.getCurrentMaxSpeed());

	  // vertical speed: jump
	  if(input.wantJump() && !isCrouch) {
		// jump
		rigidBody.velocity = new Vector2(horizontalSpeed, movementAbility.jumpInitialSpeed);
		return getStateFactory().getState<JumpState>();
	  } else if(input.wantAttack()) {
		IStatus meleeStatus = getStateFactory().getState<MeleeStatus>();
		if(meleeStatus.isEligible()) {
		  return meleeStatus;
		}
	  } else if(input.wantShoot() && !isCrouch) {
		IStatus shootStatus = getStateFactory().getState<ShootStatus>();
		if(shootStatus.isEligible()) {
		  return shootStatus;
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

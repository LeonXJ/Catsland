using UnityEngine;
using System.Collections.Generic;

namespace Catslandx {
  public class MovementState :AbstractState {

    private MovementAbility movementAbility;
    private Rigidbody2D rigidBody;

    private bool crouch;

    public MovementState(GameObject gameObject) : base(gameObject) {
      movementAbility = getComponent<MovementAbility>();
      rigidBody = getComponent<Rigidbody2D>();
    }

    public override void onEnter(IState previousState) {}

    public override void onExit(IState nextState) {}

    public override IState update(Dictionary<SensorEnum, ISensor> sensors, ICharacterInput input, float deltaTime) {
      // check ground sensor
      ISensor groundSensor = sensors[SensorEnum.ON_GROUND_SENSOR];
      if (groundSensor == null || !groundSensor.isTrigger()) {
        // not on ground
        // return jump state
      }
      // horizontal speed: crouch or run 
      float maxHorzontalSpeed = movementAbility.maxRunSpeed;
      // check ceiling
      ISensor ceilingSensor = sensors[SensorEnum.CEILING_SENSOR];
      if((ceilingSensor != null && ceilingSensor.isTrigger())
        || input.wantDirection().y < movementAbility.crouchControlSensitive) {
        // something on the ceiling, need to crouch
        crouch = true;
        maxHorzontalSpeed = movementAbility.maxCrouchSpeed;
      } else {
        crouch = false;
      }
      float horizontalSpeed = input.wantDirection().x * maxHorzontalSpeed;
      float verticalSpeed = 0.0f;
      // vertical speed: jump
      bool wantJump = false;
      if(input.wantJump()) {
        // jump
        verticalSpeed = movementAbility.jumpInitialSpeed;
        wantJump = true;
      }
      rigidBody.velocity = new Vector2(horizontalSpeed, verticalSpeed);
      if(input.wantJump()) {
        // return jump state
      }
      return this;
    }
  }
}

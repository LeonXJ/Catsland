using UnityEngine;
using System.Collections.Generic;

namespace Catslandx {
  public class JumpState :AbstractState{

    private MovementAbility movementAbility;
    private Rigidbody2D rigidBody;

    public JumpState(GameObject gameObject) : base(gameObject) {
      movementAbility = getComponent<MovementAbility>();
      rigidBody = getComponent<Rigidbody2D>();
    }

    public override void onEnter(IState previousState) { }

    public override void onExit(IState nextState) { }

    public override IState update(Dictionary<SensorEnum, ISensor> sensors, ICharacterInput input, float deltaTime) {
      // check ground sensor
      ISensor groundSensor = sensors[SensorEnum.ON_GROUND_SENSOR];
      if (groundSensor != null && groundSensor.isTrigger()) {
        // on ground
        return new MovementState(getGameObject());
      }
      // check relay
      float velticalSpeed = rigidBody.velocity.y;
      ISensor relaySensor = sensors[SensorEnum.RELAY_SENSOR];
      if (relaySensor != null && relaySensor.isTrigger()) {
        if (input.wantDash()) {
          // return dash state
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

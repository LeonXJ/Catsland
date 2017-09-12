using UnityEngine;
using System.Collections.Generic;
using Catslandx.Script.CharacterController.Common;
using Catslandx.Script.Sensor;
using Catslandx.Script.Input;

namespace Catslandx.Script.CharacterController.Ninja {

  public class HurtStatus : AbstractStatus {

    private HealthAbility healthAbility;
    private Rigidbody2D rigidBody;
    private float elapsedTimeInMs;

    public HurtStatus(GameObject gameObject, StatusFactory stateFactory)
        : base(gameObject, stateFactory) {
      healthAbility = getComponent<HealthAbility>();
      rigidBody = getComponent<Rigidbody2D>();
    }

    public override void onEnter(IStatus previousStatus) {
      base.onEnter(previousStatus);
      elapsedTimeInMs = 0.0f;
    }

    public override IStatus update(
        Dictionary<SensorEnum, ISensor> sensors,
        ICharacterInput input,
        float deltaTime) {

      elapsedTimeInMs += deltaTime;
      float frezeTimeInMs = healthAbility.freezeInMs;
      if(elapsedTimeInMs > frezeTimeInMs) {
        return getStateFactory().getState<MovementStatus>();
      }
      return this;
    }
  }
}


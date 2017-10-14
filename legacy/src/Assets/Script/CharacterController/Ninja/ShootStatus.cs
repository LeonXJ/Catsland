using System.Collections.Generic;
using UnityEngine;
using Catslandx.Script.CharacterController.Common;
using Catslandx.Script.Input;
using Catslandx.Script.Sensor;

namespace Catslandx.Script.CharacterController.Ninja {

  // TODO: abstract common part with MeleeAbility
  /** Shoot status for character controller.
   * 
   * RequiredL
   * - ShootAbility
   */ 
  public class ShootStatus : AbstractStatus {

    private ShootAbility shootAbility;

    private ShootAbility.ShootSubstatus currentShootSubstatus = ShootAbility.ShootSubstatus.STANDBY;
    private float elapsedTimeInCurrentSubstatusInMs = 0.0f;

    public ShootStatus(GameObject gameObject, StatusFactory statusFactory) : base(gameObject, statusFactory) {
      this.shootAbility = getComponent<ShootAbility>();
    }

    public override bool isEligible() {
      return shootAbility != null && currentShootSubstatus == ShootAbility.ShootSubstatus.STANDBY;
    }

	public override void onExit(IStatus nextStatus) {
	  currentShootSubstatus = ShootAbility.ShootSubstatus.STANDBY;
	}

	public override IStatus update(Dictionary<SensorEnum, ISensor> sensors, ICharacterInput input, float deltaTime) {

      float newElapsedTimeInMs = elapsedTimeInCurrentSubstatusInMs + deltaTime;
      float timeForCurrentStatusInMs = shootAbility.getSubstatusTimeInMs(currentShootSubstatus);

      while(newElapsedTimeInMs > timeForCurrentStatusInMs) {
        newElapsedTimeInMs -= timeForCurrentStatusInMs;
        // next stage
        switch(currentShootSubstatus) {
          case ShootAbility.ShootSubstatus.STANDBY:
            // do nothing
            currentShootSubstatus = ShootAbility.ShootSubstatus.PREPARE;
            break;
          case ShootAbility.ShootSubstatus.PREPARE:
            doShoot();
            currentShootSubstatus = ShootAbility.ShootSubstatus.PERFORM;
            break;
          case ShootAbility.ShootSubstatus.PERFORM:
            doFinish();
            return getStateFactory().getState<MovementStatus>();
        }
        timeForCurrentStatusInMs = shootAbility.getSubstatusTimeInMs(currentShootSubstatus);
      }
      elapsedTimeInCurrentSubstatusInMs = newElapsedTimeInMs;
      return this;
    }

    private void doShoot() {
      shootAbility.createBulletGO();
    }

    private void doFinish() {
      elapsedTimeInCurrentSubstatusInMs = 0.0f;
      currentShootSubstatus = ShootAbility.ShootSubstatus.STANDBY;
    }

  }
}
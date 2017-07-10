using UnityEngine;
using Catslandx.Script.CharacterController.Common;
using System.Collections.Generic;
using Catslandx.Script.Sensor;
using Catslandx.Script.Input;

namespace Catslandx.Script.CharacterController.Ninja {

  /** Melee status for character controller.
   * 
   * Required:
   * - AttackAbility
   */
  public class MeleeStatus :AbstractStatus {

    private MeleeAbility attackAbility;
    private Rigidbody2D rigidBody;

    private MeleeAbility.AttackSubstatus currentAttackSubstatus = MeleeAbility.AttackSubstatus.STANDBY;
    private float elapsedTimeInCurrentSubstatusInMs = 0.0f;

    public MeleeStatus(GameObject gameObject, StatusFactory stateFactory) : base(gameObject, stateFactory) {
      attackAbility = getComponent<MeleeAbility>();
      rigidBody = getComponent<Rigidbody2D>();
    }

    public override bool isEligible() {
      return attackAbility != null && currentAttackSubstatus == MeleeAbility.AttackSubstatus.STANDBY;
    }

    public override IStatus update(
        Dictionary<SensorEnum, ISensor> sensors,
        ICharacterInput input,
        float deltaTime) {
      
      rigidBody.velocity = new Vector2(0.0f, rigidBody.velocity.y);
      float newElapsedTimeInMs = elapsedTimeInCurrentSubstatusInMs + deltaTime;
      float timeForCurrentStatusInMs = attackAbility.getSubstatusTimeInMs(currentAttackSubstatus);

      while(newElapsedTimeInMs > timeForCurrentStatusInMs) {
        newElapsedTimeInMs -= timeForCurrentStatusInMs;
        // next stage
        switch(currentAttackSubstatus) {
          case MeleeAbility.AttackSubstatus.STANDBY:
            // do nothing
            currentAttackSubstatus = MeleeAbility.AttackSubstatus.PREPARE;
            break;
          case MeleeAbility.AttackSubstatus.PREPARE:
            doMelee();
            currentAttackSubstatus = MeleeAbility.AttackSubstatus.PERFORM;
            break;
          case MeleeAbility.AttackSubstatus.PERFORM:
            doFinish();
            return getStateFactory().getState<MovementStatus>();
        }
        timeForCurrentStatusInMs = attackAbility.getSubstatusTimeInMs(currentAttackSubstatus);
      }
      elapsedTimeInCurrentSubstatusInMs = newElapsedTimeInMs;
      return this;
    }

    private void doMelee() {
      attackAbility.createMeleeGO();
    }

    private void doFinish() {
      elapsedTimeInCurrentSubstatusInMs = 0.0f;
      currentAttackSubstatus = MeleeAbility.AttackSubstatus.STANDBY;
    }
  }
}

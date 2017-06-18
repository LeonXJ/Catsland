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

    public MeleeStatus(GameObject gameObject, StatusFactory stateFactory) : base(gameObject, stateFactory) {
      attackAbility = getComponent<MeleeAbility>();
    }

    public override bool isEligible() {
      return attackAbility != null && attackAbility.getMeleeStage() == MeleeAbility.AttackStage.STANDBY;
    }

    public override IStatus update(
        Dictionary<SensorEnum, ISensor> sensors,
        ICharacterInput input,
        float deltaTime) {
      MeleeAbility.AttackStage stage = attackAbility.getMeleeStage();
      float remainingTime = deltaTime;
      float stageElipsisInMs = attackAbility.getStageElipsisInMs();
      while(remainingTime > 0.0f) {
        float stageTimeInMs = attackAbility.getStageTimeInMs(stage);
        remainingTime = stageElipsisInMs + remainingTime - stageTimeInMs;
        if(remainingTime > 0.0f) {
          // next stage
          switch(stage) {
            case MeleeAbility.AttackStage.STANDBY:
              // do nothing
              stage = MeleeAbility.AttackStage.PREPARE;
              break;
            case MeleeAbility.AttackStage.PREPARE:
              doMelee();
              stage = MeleeAbility.AttackStage.PERFORM;
              break;
            case MeleeAbility.AttackStage.PERFORM:
              doFinish();
              stage = MeleeAbility.AttackStage.FINISH;
              break;
            case MeleeAbility.AttackStage.FINISH:
            case MeleeAbility.AttackStage.COOLDOWN:
              // Leave Cooldown to AttackAbility
              attackAbility.setStageElipsisInMs(remainingTime);
              attackAbility.setMeleeStage(MeleeAbility.AttackStage.COOLDOWN);
              return getStateFactory().getState<MovementStatus>();
          }
        } else {
          stageElipsisInMs = -remainingTime;
        }
      }
      attackAbility.setStageElipsisInMs(stageElipsisInMs);
      attackAbility.setMeleeStage(stage);
      return this;
    }

    private void doMelee() {
      System.Console.Out.WriteLine("DEBUG >>> doMelee");
    }

    private void doFinish() {
      System.Console.Out.WriteLine("DEBUG >>> doFinish");
    }
  }
}

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

    public MeleeStatus(GameObject gameObject, StatusFactory stateFactory) : base(gameObject, stateFactory) {
      attackAbility = getComponent<MeleeAbility>();
      rigidBody = getComponent<Rigidbody2D>();
    }

    public override bool isEligible() {
      return attackAbility != null && attackAbility.getMeleeStage() == MeleeAbility.AttackStage.STANDBY;
    }

    public override IStatus update(
        Dictionary<SensorEnum, ISensor> sensors,
        ICharacterInput input,
        float deltaTime) {
      
      rigidBody.velocity = new Vector2(0.0f, rigidBody.velocity.y);
      MeleeAbility.AttackStage stage = attackAbility.getMeleeStage();
      float stageElipsisInMs = attackAbility.getStageElipsisInMs() + deltaTime;
      float stageTimeInMs = attackAbility.getStageTimeInMs(stage);
      while(stageElipsisInMs > stageTimeInMs) {
        stageElipsisInMs -= stageTimeInMs;
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
              attackAbility.setStageElipsisInMs(stageElipsisInMs);
              attackAbility.setMeleeStage(MeleeAbility.AttackStage.COOLDOWN);
              return getStateFactory().getState<MovementStatus>();
          }
        stageTimeInMs = attackAbility.getStageTimeInMs(stage);
      }
      attackAbility.setStageElipsisInMs(stageElipsisInMs);
      attackAbility.setMeleeStage(stage);
      return this;
    }

    private void doMelee() {
      System.Console.Out.WriteLine("DEBUG >>> doMelee");
      GameObject prototype = attackAbility.getMeleePrototype();
      GameObject meleeObject = GameObject.Instantiate(prototype, getGameObject().transform);
      meleeObject.transform.position = getGameObject().transform.position;
    }

    private void doFinish() {
      System.Console.Out.WriteLine("DEBUG >>> doFinish");
    }
  }
}

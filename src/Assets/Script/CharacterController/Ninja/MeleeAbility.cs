using UnityEngine;
using System.Collections;
using Catslandx.Script.CharacterController.Common;
using System.Collections.Generic;

namespace Catslandx.Script.CharacterController.Ninja {
  public class MeleeAbility :AbstractCharacterAbility {

    public enum AttackStage {
      STANDBY,
      PREPARE,
      PERFORM,
      FINISH,
      COOLDOWN
    }

    public float meleePrepreInMs;
    public float meleePerformInMs;
    public float meleeFinishInMs;
    public float meleeCooldownInMs;

    private AttackStage meleeStage;
    private float stageElipsisInMs;

    public AttackStage getMeleeStage() {
      return meleeStage;
    }

    public void setMeleeStage(AttackStage attackStage) {
      meleeStage = attackStage;
    }

    public float getStageElipsisInMs() {
      return stageElipsisInMs;
    }

    public void setStageElipsisInMs(float stageElipsisInMs) {
      this.stageElipsisInMs = stageElipsisInMs;
    }

    public float getStageTimeInMs(AttackStage attackStage) {
      switch(attackStage) {
        case AttackStage.PREPARE:
          return meleePrepreInMs;
        case AttackStage.PERFORM:
          return meleePerformInMs;
        case AttackStage.FINISH:
          return meleeFinishInMs;
        case AttackStage.COOLDOWN:
          return meleeCooldownInMs;
        default:
          return 0.0f;
      }
    }

    public void Update() {
      float time = Time.deltaTime;
      if(meleeStage == AttackStage.COOLDOWN) {
        stageElipsisInMs += time;
        if(stageElipsisInMs > meleeCooldownInMs) {
          resetMelee();
        }
      }
    }

    private void resetMelee() {
      stageElipsisInMs = 0.0f;
      meleeStage = AttackStage.STANDBY;
    }
  }
}

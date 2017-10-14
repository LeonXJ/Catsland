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
	private bool nextAttack = false;
	private Orientation nextAttackOrientation;
	private int meleeLevel = 0;

	public MeleeStatus(GameObject gameObject, StatusFactory stateFactory) : base(gameObject, stateFactory) {
	  attackAbility = getComponent<MeleeAbility>();
	  rigidBody = getComponent<Rigidbody2D>();
	}

	public override bool isEligible() {
	  return attackAbility != null && currentAttackSubstatus == MeleeAbility.AttackSubstatus.STANDBY;
	}

	public override void onEnter(IStatus previousStatus) {
	  base.onEnter(previousStatus);
	  nextAttack = false;
	  meleeLevel = 0;
	  nextAttackOrientation = characterController.getOrientation();
	}

	public override void onExit(IStatus nextStatus) {
	  currentAttackSubstatus = MeleeAbility.AttackSubstatus.STANDBY;
	}

	public override IStatus update(
		Dictionary<SensorEnum, ISensor> sensors,
		ICharacterInput input,
		float deltaTime) {

	  // want next attack
	  if(input.wantAttack()) {
		nextAttack = true;
	  }
	  float wantDirectionX = input.wantDirection().x;
	  if(Mathf.Abs(wantDirectionX) > Mathf.Epsilon) {
		nextAttackOrientation = wantDirectionX > 0.0f ? Orientation.Right : Orientation.Left;
	  }

	  //rigidBody.velocity = new Vector2(0.0f, rigidBody.velocity.y);
	  float newElapsedTimeInMs = elapsedTimeInCurrentSubstatusInMs + deltaTime;
	  float timeForCurrentStatusInMs = attackAbility.getSubstatusTimeInMs(currentAttackSubstatus);

	  while(newElapsedTimeInMs > timeForCurrentStatusInMs) {
		newElapsedTimeInMs -= timeForCurrentStatusInMs;
		// next stage
		switch(currentAttackSubstatus) {
		  case MeleeAbility.AttackSubstatus.STANDBY:
			// can adjust orientation
			characterController.setOrientation(nextAttackOrientation);
			currentAttackSubstatus = MeleeAbility.AttackSubstatus.PREPARE;
			break;
		  case MeleeAbility.AttackSubstatus.PREPARE:
			doMelee();
			currentAttackSubstatus = MeleeAbility.AttackSubstatus.PERFORM;
			break;
		  case MeleeAbility.AttackSubstatus.PERFORM:
			doFinish();
			if(nextAttack) {
			  nextAttack = false;
			  meleeLevel = meleeLevel == 0 ? 1 : 0;
			  return this;
			}
			return getStateFactory().getState<MovementStatus>();
		}
		timeForCurrentStatusInMs = attackAbility.getSubstatusTimeInMs(currentAttackSubstatus);
	  }
	  elapsedTimeInCurrentSubstatusInMs = newElapsedTimeInMs;
	  return this;
	}

	public override void applyAnimation(Animator animator) {
	  animator.SetInteger(NinjaController.AnimatorValue.meleeLevel, meleeLevel);

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

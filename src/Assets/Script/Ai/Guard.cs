using UnityEngine;
using Catslandx.Script.Input;
using Catslandx.Script.Ai.Node;
using Catslandx.Script.CharacterController;
using System;

namespace Catslandx.Script.Ai {
  public class Guard :MonoBehaviour, ICharacterInput {

	enum Status {
	  Patrol,
	  Persue,
	}

	// patrol
	public PatrolInitializer patrolInitializer;

	// detect
	public Vector2 detectRectangle = new Vector2(3.0f, 1.0f);

	// attack
	public float meleeRang = 0.2f;

	private AiNode patrolNode;

	private Status status = Status.Patrol;
	private int currentPatrolActionIdx = 0;

	private ICharacterController characterController;
	private GameObject player;

	private Vector2 intentDirection = Vector2.zero;
	private bool isIntentMelee = false;
	private bool isIntentShoot = false;

	private long lastTick = 0;

	private void clearIntent() {
	  intentDirection = Vector2.zero;
	  isIntentMelee = false;
	  isIntentShoot = false;
	}

	public void intentLeft() {
	  intentDirection = new Vector2(-1.0f, 0);
	}

	public void intentRight() {
	  intentDirection = new Vector2(1.0f, 0);
	}

	public void intentShoot() {
	  isIntentShoot = true;
	}

	public void intentMelee() {
	  isIntentMelee = true;
	}

	public void updateInput(float deltaTime) {
	  float delta = Time.deltaTime;
	  long currentTick = lastTick + 1;
	  clearIntent();
	  status = Status.Patrol;
	  switch(status) {
		case Status.Patrol:
		  patrolNode.update(lastTick, currentTick, this, gameObject, delta);
		  break;
		case Status.Persue:
		  updatePersue(delta);
		  break;
	  }
	  lastTick = currentTick;
	}

	public bool wantAttack() {
	  return isIntentMelee;
	}

	public bool wantDash() {
	  return false;
	}

	public Vector2 wantDirection() {
	  return intentDirection;
	}

	public bool wantInteract() {
	  return false;
	}

	public bool wantJump() {
	  return false;
	}

	public bool wantShoot() {
	  return isIntentShoot;
	}

	private bool playerTracker() {
	  if(player != null) {
		// only find forward within distance player
		// too far
		Vector3 deltaPosition = player.transform.position - transform.position;
		if(Mathf.Abs(deltaPosition.x) > detectRectangle.x ||
		  Mathf.Abs(deltaPosition.y) > detectRectangle.y / 2.0f) {
		  return false;
		}
		// looking at the other side
		if(characterController.getOrientation() == Orientation.Left && deltaPosition.x > 0.0f
		  || characterController.getOrientation() == Orientation.Right && deltaPosition.x < 0.0f) {
		  return false;
		}
		return true;
	  }
	  return false;
	}

	private void updatePersue(float delta) {
	  Vector3 deltaPosition = player.transform.position - transform.position;
	  if(Mathf.Abs(deltaPosition.x) < meleeRang) {
		isIntentMelee = true;
	  } else {
		isIntentShoot = true;
	  }
	}

	// Use this for initialization
	void Start() {
	  if(patrolInitializer != null) {
		patrolNode = patrolInitializer.initialize();
	  }
	  player = GameObject.FindGameObjectWithTag("Player");
	  characterController = GetComponent<ICharacterController>();
	}

	// Update is called once per frame
	void Update() {
	}
  }
}

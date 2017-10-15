using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.CharacterController {

  [RequireComponent(typeof(IInput))]
  public class PlayerController :MonoBehaviour {

	// Locomoation
	public float maxHorizontalSpeed = 1.0f;
	public float acceleration = 1.0f;


	// References
	private IInput input;
	private Rigidbody2D rb2d;

	public void Awake() {
	  input = GetComponent<IInput>();
	  rb2d = GetComponent<Rigidbody2D>();
	}

	public void Update() {
	  float desiredSpeed = input.getHorizontal();

	  if(Mathf.Abs(desiredSpeed) > Mathf.Epsilon) {
		rb2d.AddForce(new Vector2(acceleration * desiredSpeed, 0.0f));
		rb2d.velocity = new Vector2(
			Mathf.Clamp(rb2d.velocity.x, -maxHorizontalSpeed, maxHorizontalSpeed),
			rb2d.velocity.y);
	  } else {
		rb2d.velocity = new Vector2(0.0f, rb2d.velocity.y);
	  }


	}
  }
}

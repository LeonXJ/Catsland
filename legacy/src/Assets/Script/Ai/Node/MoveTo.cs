using UnityEngine;

namespace Catslandx.Script.Ai.Node {
  public class MoveToX : AiNode {

	private float arriveDistance;
	private float destinationX;

	public MoveToX(float destinationX, float arriveDistance = 0.1f) {
	  this.destinationX = destinationX;
	  this.arriveDistance = arriveDistance;
	}

	public bool update(
	  long lastTick, long currentTick, Guard input, GameObject character, float delta) {
	  float deltaPosition = destinationX - character.transform.position.x;
	  if(deltaPosition * deltaPosition < arriveDistance * arriveDistance) {
		return true;
	  }

	  if(deltaPosition > 0.0f) {
		input.intentRight();
	  } else {
		input.intentLeft();
	  }
	  return false;
	}
  }
}

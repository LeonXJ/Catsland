namespace Catslandx.Script.CharacterController.Common {

  public class CharacterHelper {
	// Calculates the horizontal speed.
	public static float getHorizontalSpeed(
	  float inputSpeed, Orientation orientation, bool isFrontBlocked, bool isRearBlocked, float maxSpeed) {
	  // gonna move forward
	  if((inputSpeed > 0.0f && orientation == Orientation.Right)
		|| (inputSpeed < 0.0f && orientation == Orientation.Left)) {
		return isFrontBlocked ? 0.0f : inputSpeed * maxSpeed;
	  }
	  // gonna move backward
	  return isRearBlocked ? 0.0f : inputSpeed * maxSpeed;
	}
  }
}


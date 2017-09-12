using UnityEngine;

namespace Catslandx.Script.Ai.Node {
  public class Shoot: AiNode{

	public bool update(
	  long lastTick, long currentTick, Guard input, GameObject character, float delta) {
	  input.intentShoot();
	  return true;
	}
  }
}

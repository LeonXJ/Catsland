using UnityEngine;

namespace Catslandx.Script.Ai.Node {
  public class Melee: AiNode{

	public bool update(
	  long lastTick, long currentTick, Guard input, GameObject character, float delta) {
	  input.intentMelee();
	  return true;
	}
  }
}

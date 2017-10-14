using UnityEngine;

namespace Catslandx.Script.Ai.Node {
  public interface AiNode {
	bool update(long lastTick, long currentTick, Guard guard, GameObject gameObject, float deltaTime);
  }
}

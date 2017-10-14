using UnityEngine;

namespace Catslandx.Script.Ai.Node {
  public abstract class AbstractCondition :AiNode {

	protected AiNode subNode;

	public AbstractCondition(AiNode subNode) {
	  this.subNode = subNode;
	}

	public abstract bool update(
	  long lastTick, long currentTick, Guard guard, GameObject gameObject, float deltaTime);
  }
}

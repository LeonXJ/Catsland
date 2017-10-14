using UnityEngine;
using Catslandx.Script.CharacterController;

namespace Catslandx.Script.Ai.Node {
  public class IsPlayInScope :AbstractCondition {

	private Vector2 detectRectangle;
	private GameObject playerCache;
	private ICharacterController controllerCache;
	private bool onEnterAwait;
	private bool onExitAwait;

	public IsPlayInScope(AiNode subNode, Vector2 detectRectangle) : base(subNode) {
	  this.detectRectangle = detectRectangle;
	  this.onEnterAwait = false;
	}

	public override bool update(
	  long lastTick, long currentTick, Guard guard, GameObject gameObject, float deltaTime) {
	  if(playerCache == null) {
		playerCache = GameObject.FindGameObjectWithTag("Player");
	  }
	  if(playerCache != null) {
		// only find forward within distance player
		// too far
		Vector3 deltaPosition = playerCache.transform.position - gameObject.transform.position;
		if(Mathf.Abs(deltaPosition.x) > detectRectangle.x ||
		  Mathf.Abs(deltaPosition.y) > detectRectangle.y / 2.0f) {
		  // condition false, done with this node
		  return true;
		}
		// looking at the other side
		if(controllerCache == null) {
		  controllerCache = gameObject.GetComponent<ICharacterController>();
		}
		if(controllerCache.getOrientation() == Orientation.Left && deltaPosition.x > 0.0f
		  || controllerCache.getOrientation() == Orientation.Right && deltaPosition.x < 0.0f) {
		  // condition false, done with this node
		  return true;
		}
		return subNode.update(lastTick, currentTick, guard, gameObject, deltaTime);
	  }
	  // condition false, done with this node
	  return true;
	}
  }
}

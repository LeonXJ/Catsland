using System;
using UnityEngine;

namespace Catslandx.Script.Ai.Node {

  /** An AI action. */
  public abstract class AbstractWithEnterAiNode: AiNode {

	private long lastTick = -1;

	protected abstract void onEnter();

	protected abstract bool update(Guard input, GameObject character, float delta);

	/** Update the action.
	 * 
	 * Return whether this action finished.
	 */
	public bool update(
	  long lastTick, long currentTick, Guard input, GameObject character, float delta) {
	  if(this.lastTick != lastTick) {
		onEnter();
	  }
	  this.lastTick = currentTick;
	  return update(input, character, delta);
	}
  }
}
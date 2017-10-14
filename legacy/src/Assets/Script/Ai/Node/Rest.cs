using UnityEngine;

namespace Catslandx.Script.Ai.Node {
  public class Rest :AbstractWithEnterAiNode {

	private float duration;
	private float remainTime;

	public Rest(float duration) {
	  this.duration = duration;
	}

	protected override void onEnter() {
	  remainTime = duration;
	}

	protected override bool update(Guard input, GameObject character, float delta) {
	  remainTime -= delta;
	  return remainTime < 0.0f;
	}
  }
}


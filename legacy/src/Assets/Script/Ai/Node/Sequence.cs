using System.Collections.Generic;
using UnityEngine;

namespace Catslandx.Script.Ai.Node {

  public class Sequence :AiNode {

	private List<AiNode> aiNodes;
	private int currentIndex;
	private bool continueLast;
	private bool loop;

	public Sequence(bool continueLast = false, bool loop = false) {
	  aiNodes = new List<AiNode>();
	  this.continueLast = continueLast;
	  this.loop = loop;
	  currentIndex = 0;
	}

	public Sequence(List<AiNode> aiNodes, bool continueLast = false, bool loop = false) {
	  this.aiNodes = aiNodes;
	  this.continueLast = continueLast;
	  this.loop = loop;
	  currentIndex = 0;
	}

	public Sequence addNode(AiNode aiNode) {
	  aiNodes.Add(aiNode);
	  return this;
	}

	public void onEnter() {
	  if(!continueLast) {
		currentIndex = 0;
	  }
	}

	public bool update(
	  long lastTick, long currentTick, Guard guard, GameObject gameObject, float deltaTime) {
	  if(aiNodes.Count > 0) {
		if(aiNodes[currentIndex].update(lastTick, currentTick, guard, gameObject, deltaTime)) {
		  currentIndex++;
		  if(currentIndex >= aiNodes.Count) {
			currentIndex = 0;
			if(!loop) {
			  return true;
			}
		  }
		}
		return false;
	  }
	  return true;
	}
  }
}

using UnityEngine;
using System.Collections.Generic;

namespace Catslandx.Script.Ai.Node {
  public class ParallelAll :AiNode {

	private List<AiNode> aiNodes;

	public ParallelAll() {
	  aiNodes = new List<AiNode>();
	}

	public ParallelAll(List<AiNode> aiNodes) {
	  this.aiNodes = aiNodes;
	}

	public ParallelAll addNode(AiNode aiNode) {
	  aiNodes.Add(aiNode);
	  return this;
	}

	public bool update(
	  long lastTick, long currentTick, Guard guard, GameObject gameObject, float deltaTime) {

	  if(aiNodes == null) {
		return true;
	  }

	  foreach(AiNode aiNode in aiNodes) {
		if(!aiNode.update(lastTick, currentTick, guard, gameObject, deltaTime)) {
		  return false;
		}
	  }
	  return true;
	}
  }
}
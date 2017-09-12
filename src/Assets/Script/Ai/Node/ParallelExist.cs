using UnityEngine;
using System.Collections.Generic;

namespace Catslandx.Script.Ai.Node {
  public class ParallelExist :AiNode {

	private List<AiNode> aiNodes = new List<AiNode>();

	public ParallelExist(List<AiNode> aiNodes) {
	  this.aiNodes = aiNodes;
	}

	public bool update(
	  long lastTick, long currentTick, Guard guard, GameObject gameObject, float deltaTime) {

	  if(aiNodes == null) {
		return true;
	  }

	  foreach(AiNode aiNode in aiNodes) {
		if(aiNode.update(lastTick, currentTick, guard, gameObject, deltaTime)) {
		  return true;
		}
	  }
	  return false;
	}
  }
}

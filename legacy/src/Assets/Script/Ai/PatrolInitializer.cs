using UnityEngine;
using Catslandx.Script.Ai.Node;
using System.Collections.Generic;

namespace Catslandx.Script.Ai {
  public class PatrolInitializer :MonoBehaviour {

	// patrol
	public Vector2[] xAndRestDurations;

	// detect
	public Vector2 detectRectangle = new Vector2(3.0f, 1.0f);
	public float shootDurationS = 2.0f;
	public Vector2 meleeRectangle = new Vector2(0.2f, 1.0f);
	public float meleeDurationS = 1.0f;

	public AiNode initialize() {
	  List<AiNode> parallelAll = new List<AiNode>();

	  parallelAll.Add(createAttack());
	  AiNode patrol = createPatrol();
	  if(patrol != null) {
		parallelAll.Add(patrol);
	  }
	  return new ParallelAll(parallelAll);
	}

	private AiNode createPatrol() {
	  if(xAndRestDurations != null) {
		List<AiNode> actions = new List<AiNode>();
		foreach(Vector2 xAndRestDuration in xAndRestDurations) {
		  actions.Add(new MoveToX(xAndRestDuration.x));
		  if(xAndRestDuration.y > Mathf.Epsilon) {
			actions.Add(new Rest(xAndRestDuration.y));
		  }
		}
		return new Sequence(actions, true, true);
	  }
	  return null;
	}

	private AiNode createAttack() {
	  return new ParallelAll()
		.addNode(new IsPlayInScope(
		  new Sequence(false, true)
			.addNode(new Node.Melee())
			.addNode(new Rest(meleeDurationS)),
		  meleeRectangle))
		.addNode(new IsPlayInScope(
		  new Sequence(false, true)
			.addNode(new Shoot())
			.addNode(new Rest(shootDurationS)),
		  detectRectangle));
	}

	void OnDrawGizmosSelected() {
	  if(xAndRestDurations != null) {
		UnityEditor.Handles.color = Color.red;
		foreach(Vector2 xAndRestDuration in xAndRestDurations) {
		  UnityEditor.Handles.DrawLine(new Vector3(xAndRestDuration.x, 100.0f), new Vector3(xAndRestDuration.x, -100f));
		}
	  }
	}
  }
}

using UnityEngine;
using System.Collections.Generic;

namespace Catslandx.Script.Sensor {

  /** A componenet which implements IPreCalculateSensor. */
  public class PreCalculateSensor :MonoBehaviour, IPreCalculateSensor {

	// Whitelist of the collide layers
	public LayerMask collideTypes;

	// The collider is a circle (so far).
	public float radius;

	// Blacklist of the collide Game Objects.
	public GameObject[] blackListGOs;

	private GameObject collideGO;
	private GameObject previousCollideGO;
	private HashSet<GameObject> blackListedGOs;
	private List<IPreCalculateSensorSubscriber> subscribers = new List<IPreCalculateSensorSubscriber>();

	void Start() {
	  if(blackListGOs != null) {
		blackListedGOs = new HashSet<GameObject>();
		foreach(GameObject go in blackListGOs) {
		  blackListedGOs.Add(go);
		}
	  }
	}

	void OnDrawGizmosSelected() {
	  UnityEditor.Handles.color = isInTrigger() ? Color.red : Color.white;
	  UnityEditor.Handles.DrawWireDisc(gameObject.transform.position, Vector3.back, radius);
	}

	public void addSubscriber(IPreCalculateSensorSubscriber subscriber) {
	  subscribers.Add(subscriber);
	}

	/** Does precaluclation and calls subscribers' update.
     * 
     * This should be called once in each cycle.
     */
	public void preCalculate() {
	  previousCollideGO = collideGO;
	  collideGO = null;
	  Collider2D[] colliders = Physics2D.OverlapCircleAll(
		transform.position, radius, collideTypes);
	  foreach(Collider2D collider in colliders) {
		if(blackListedGOs != null
		  && blackListedGOs.Contains(collider.gameObject)) {
		  continue;
		} else {
		  collideGO = collider.gameObject;
		  break;
		}
	  }
	  updateSubscribers();
	}

	public bool isOnTriggerOn() {
	  return previousCollideGO == null && collideGO != null;
	}

	public bool isInTrigger() {
	  return collideGO != null;
	}

	public bool isOnTriggerOff() {
	  return previousCollideGO != null & collideGO == null;
	}

	public GameObject getCollideGO() {
	  return collideGO;
	}

	private void updateSubscribers() {
	  foreach(IPreCalculateSensorSubscriber subscriber in subscribers) {
		subscriber.doUpdate(this);
	  }
	}
  }
}

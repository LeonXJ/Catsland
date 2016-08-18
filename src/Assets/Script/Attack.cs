using UnityEngine;
using System.Collections;

namespace Catslandx {
  public class Attack :MonoBehaviour {

    public enum Status {
      INACTIVE = 0,
      PRELOAD = 1,
      INACTION = 2,
      POSTACTION = 3,
      COOLDOWN = 4
    }

    public int attackValue = 1;
    public GameObject attackGameObjectPrototype;
    public Vector2 offset;
    public float preloadS = 0.1f;
    public float actionS = 0.1f;
    public float postActionS = 0.1f;
    public float coolDownS = 1.0f;

    public Status status {
      get {
        if (!isTriggerred) {
          return Status.INACTIVE;
        } else {
          float time = currentTimeS;
          if (time < preloadS) {
            return Status.PRELOAD;
          }
          time -= preloadS;
          if (time < actionS) {
            return Status.INACTION;
          }
          time -= actionS;
          if (time < postActionS) {
            return Status.POSTACTION;
          }
          time -= postActionS;
          if (time < coolDownS) {
            return Status.COOLDOWN;
          }
          return Status.INACTIVE;
        }
      }
    }

    public float currentTimeS = 0.0f;
    private bool isTriggerred = false;
    private bool hasAttack = false;

    public void activate(Vector2 offset) {
      isTriggerred = true;
      this.offset = offset;
    }

	void Update () {
      if (isTriggerred) {
        currentTimeS += Time.deltaTime;
        if (!hasAttack && status != Status.INACTIVE && status != Status.PRELOAD) {
          attack();
          hasAttack = true;
        }
        if (status == Status.INACTIVE) {
          reset();
        }
      }
	}

    void attack() {
      if (attackGameObjectPrototype != null) {
        GameObject attackGameObject = Instantiate(attackGameObjectPrototype);
        if (attackGameObject != null) {
          attackGameObject.transform.position = MathHelper.getXY(transform.position) + offset;
          IAttackObject attackObject = attackGameObject.GetComponent<IAttackObject>();
          if (attackObject != null) {
            attackObject.init(gameObject, attackValue);
            attackObject.activate();
          }
        }
      }
    }

    public void interrupt() {
      // if is attacking and now in cooldown, directly jump to cooldown
      if (isTriggerred && status != Status.INACTIVE && status != Status.COOLDOWN) {
        currentTimeS = preloadS + actionS + postActionS;
        hasAttack = true;
      }
    }

    public void reset() {
      isTriggerred = false;
      currentTimeS = 0.0f;
      hasAttack = false;
    }
  }
}

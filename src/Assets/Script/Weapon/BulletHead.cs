using UnityEngine;
using Catslandx.Script.Common;

[RequireComponent(typeof(Collider2D))]
public class BulletHead :MonoBehaviour {

  public bool isTemporary;
  public float lifetimeInMs;
  public bool selfDestoryAfterHit;
  public bool canHurtOwner;
  public int damage;

  private GameObject owner;
  private float age = 0.0f;
  private bool fired = false;

  public void fire(GameObject owner) {
    this.owner = owner;
    fired = true;
  }

  // Update is called once per frame
  void Update() {
    if(!fired) {
      return;
    }
    if(isTemporary) {
      age += Time.deltaTime;
      if(age > lifetimeInMs) {
        selfDestroy();
      }
    }
  }

  void OnTriggerEnter2D(Collider2D other) {
    if(!fired) {
      return;
    }
    if(other.gameObject == this.gameObject) {
      return;
    }
    if(canHurtOwner || other.gameObject != owner) {
      IVulnerable vulerable = other.GetComponent<IVulnerable>();
      if(vulerable != null) {
        vulerable.getHurt(damage, other.transform.position - gameObject.transform.position);
        if(selfDestoryAfterHit) {
          selfDestroy();
        }
      }
    }
  }

  private void selfDestroy() {
    Destroy(gameObject);
  }
}

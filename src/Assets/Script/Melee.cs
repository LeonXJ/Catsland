using UnityEngine;
using Catslandx.Script.Common;

namespace Catslandx {
  public class Melee : MonoBehaviour, IAttackObject {

    public float radius = 0.1f;
    public LayerMask targetLayer;

    private GameObject owner;
    private int attack;

    void IAttackObject.init(GameObject owner, int attack) {
      this.owner = owner;
      this.attack = attack;
    }

    void IAttackObject.activate() {
      Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);
      // attack the first one in the list
      foreach (Collider2D collider in colliders) {
        if (collider.gameObject != owner) {
          IVulnerable vulnerable = collider.gameObject.GetComponent<IVulnerable>();
          if (vulnerable != null) {
            vulnerable.getHurt(attack, collider.gameObject.transform.position - gameObject.transform.position);
            break;
          }
        }
      }
      despose();
    }

    private void despose() {
      Destroy(gameObject);
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
  }
}

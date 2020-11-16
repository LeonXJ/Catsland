using UnityEngine;
using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.Controller {
  public class Barrier : MonoBehaviour, IDamageInterceptor {

    private const string ANI_PARAM_IS_DAMAGED = "IsDamaged";

    public int health = 2;

    // Consider damage if the health is lower.
    public int damageHealthThreshold = 1;

    private Animator animator;

    // Start is called before the first frame update
    void Start() {
      animator = GetComponent<Animator>();
    }

    public void damage(DamageInfo damageInfo) {
      health -= 1;
      animator.SetBool(ANI_PARAM_IS_DAMAGED, health <= damageHealthThreshold);

      if (health <= 0) {
        GetComponent<Misc.DebrideGenerator>()?.GenerateDebrides();
        Destroy(gameObject);
      }
    }

    public ArrowResult getArrowResult(ArrowCarrier arrowCarrier) {
      return ArrowResult.HIT_AND_BROKEN;
    }
  }
}

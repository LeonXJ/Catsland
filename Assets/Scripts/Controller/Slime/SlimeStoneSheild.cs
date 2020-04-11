using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.Controller.Slime {
  public class SlimeStoneSheild : MonoBehaviour, IDamageInterceptor, IMeleeDamageInterceptor {

    private SlimeController slimeController;

    public ArrowResult getArrowResult(ArrowCarrier arrowCarrier) {
      return arrowCarrier.isShellBreaking ? ArrowResult.HIT_AND_BROKEN : ArrowResult.BROKEN;
    }

    // Start is called before the first frame update
    void Start() {
      slimeController = gameObject.transform.parent.GetComponent<SlimeController>();
    }

    // Update is called once per frame
    void Update() {

    }
    public void damage(DamageInfo damageInfo) {
      if (damageInfo.isKnockback()) {
        slimeController.damage(damageInfo);
        return;
      }
      if (damageInfo.isShellBreaking) {
        slimeController.onShieldHit();
      }
    }

    public MeleeResult getMeleeResult() {
      return MeleeResult.HIT;
    }
  }
}

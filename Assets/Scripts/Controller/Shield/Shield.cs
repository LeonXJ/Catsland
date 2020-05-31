using UnityEngine;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller.Shield {
  public class Shield : MonoBehaviour, IDamageInterceptor, IMeleeDamageInterceptor {

    public ArrowResult onNormalArrow;
    public ArrowResult onStrongArrow;
    public MeleeResultStatus onMelee;
    public GameObject shieldCallback;

    public ArrowResult getArrowResult(ArrowCarrier arrowCarrier) {
      return arrowCarrier.isShellBreaking ? onStrongArrow : onNormalArrow;
    }

    public MeleeResult getMeleeResult() {
      return new MeleeResult(onMelee);
    }

    public void damage(DamageInfo damageInfo) {
      shieldCallback?.SendMessage(
        MessageNames.ON_SHIELD_DAMAGE_FUNCTION,
        damageInfo,
        SendMessageOptions.DontRequireReceiver);
    }
  }
}

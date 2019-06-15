using UnityEngine;

namespace Catsland.Scripts.Bullets {
  public class AttachedArrow: MonoBehaviour, IDamageInterceptor {

    public GameObject brokenArrowPrefab;
    public float brokenPartBounceSpeedRatio = 0.8f;
    public float brokenPartSpinSpeed = 320f;

    public void damage(DamageInfo damageInfo) {
      BulletUtils.generateDebrid(brokenArrowPrefab, transform,
        damageInfo.repelDirection.x * brokenPartBounceSpeedRatio,
        (Random.value - 0.5f) * 2.0f * brokenPartSpinSpeed);
      Destroy(gameObject);
    }

    ArrowResult IDamageInterceptor.getArrowResult(ArrowCarrier arrowCarrier) {
      return ArrowResult.HIT_AND_BROKEN;
    }
  }
}

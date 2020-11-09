using UnityEngine;
using DG.Tweening;

namespace Catsland.Scripts.Bullets {
  public class AttachedArrow: MonoBehaviour, IDamageInterceptor {

    public GameObject brokenArrowPrefab;
    public float brokenPartBounceSpeedRatio = 0.8f;
    public float brokenPartSpinSpeed = 320f;
    public float repelVelocityScale = 1.2f;
    public float brokenPartDrag = .3f;

    [Header("Shaking Effect")]
    public float attachShakeTime = .1f;
    public float attachShakePositionStrength = .1f;
    public float attachShakeRotationStrength = 10f;

    public float kickedShakeTime = .2f;
    public float kickedShakeRotationStrength = 30f;

    public void damage(DamageInfo damageInfo) {
      BulletUtils.generateDebrid(brokenArrowPrefab, transform,
        damageInfo.GetRepelVelocity() * repelVelocityScale,
        (Random.value - 0.5f) * 2.0f * brokenPartSpinSpeed,
        brokenPartDrag) ;
      Destroy(gameObject);
    }

    public void Shake() {
      transform.DOShakePosition(attachShakeTime, gameObject.transform.right * attachShakePositionStrength, 30, 120);
      transform.DOShakeRotation(attachShakeTime, new Vector3(0f, 0f, attachShakeRotationStrength), 30, 120);
    }

    public void OnRelayKicked() {
      transform.DOShakeRotation(kickedShakeTime, new Vector3(0f, 0f, kickedShakeRotationStrength), 50, 120);
    }

    ArrowResult IDamageInterceptor.getArrowResult(ArrowCarrier arrowCarrier) {
      return ArrowResult.HIT_AND_BROKEN;
    }
  }
}

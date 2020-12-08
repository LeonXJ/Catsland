using UnityEngine;
using DG.Tweening;
using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.Controller {
  public class Barrier : MonoBehaviour, IDamageInterceptor {

    private const string ANI_PARAM_IS_DAMAGED = "IsDamaged";

    public int health = 2;
    public Timing timing;

    // Consider damage if the health is lower.
    public int damageHealthThreshold = 1;

    public GameObject[] notifyGameObjectsWhenDestroy;

    private Animator animator;
    private CharacterEventSounds characterEventSounds;

    // Start is called before the first frame update
    void Start() {
      animator = GetComponent<Animator>();
      characterEventSounds = GetComponent<CharacterEventSounds>();
    }

    public void damage(DamageInfo damageInfo) {
      health -= 1;
      animator.SetBool(ANI_PARAM_IS_DAMAGED, health <= damageHealthThreshold);
      characterEventSounds.PlayOnDamageSound();
      transform.DOShakePosition(timing.ArrowShakeTime, .15f, 30, 120);

      if (health <= 0) {
        GetComponent<Misc.DebrideGenerator>()?.GenerateDebrides();
        characterEventSounds.PlayOnDieSound();
        NotifyGameObjects();
        Destroy(gameObject);
      }
    }

    private void NotifyGameObjects() {
      if (notifyGameObjectsWhenDestroy != null) {
        foreach (var gameObject in notifyGameObjectsWhenDestroy) {
          gameObject.SendMessage(
            Common.MessageNames.ON_DESTROYED,
            new Common.OnDestroyedInfo(gameObject.name),
            SendMessageOptions.DontRequireReceiver);
        }
      }
    }

    public ArrowResult getArrowResult(ArrowCarrier arrowCarrier) {
      return ArrowResult.HIT_AND_BROKEN;
    }
  }
}

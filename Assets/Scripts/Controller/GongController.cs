using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Misc;

namespace Catsland.scripts.controller {
  public class GongController : MonoBehaviour, Catsland.Scripts.Bullets.IDamageInterceptor  {

    public float shakeInS = .2f;

    public ArenaDirector arenaDirector;

    public void damage(DamageInfo damageInfo) {
      transform.DOShakePosition(shakeInS, .15f, 30, 120);

      generateRippleEffect();

      arenaDirector?.PlayFromBeginning();
    }

    public ArrowResult getArrowResult(ArrowCarrier arrowCarrier) {
      return ArrowResult.HIT_AND_BROKEN;
    }

    private void generateRippleEffect() {
      RippleEffect rippleEffect = Camera.main.gameObject.GetComponent<RippleEffect>();

      rippleEffect.Emit(Camera.main.WorldToViewportPoint(transform.position));
    }
  }
}

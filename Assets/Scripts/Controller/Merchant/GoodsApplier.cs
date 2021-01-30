using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller.Merchant {
  public class GoodsApplier : MonoBehaviour {

    public enum GoodsEffect {
      UNKNOWN = 0,
      RECOVER_HEALTH = 1,
    }

    public ParticleSystem effectParticle;

    public GoodsEffect effect;
    public int[] parameters;

    public void PlayParticleAndApply() {
      PlayParticle();
      Apply();
    }

    public void PlayParticle() {
      effectParticle?.Play();
    }

    public void Apply() {
      switch (effect) {
        case GoodsEffect.RECOVER_HEALTH:
        ApplyRecoverHealth();
        break;
      }
    }

    private void ApplyRecoverHealth() {
      GameObject.FindGameObjectWithTag(Common.Tags.PLAYER).GetComponent<PlayerController>().RecoverHealth(parameters[0]);
    }
  }
}

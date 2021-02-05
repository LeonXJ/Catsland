using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;
using Cinemachine;
using DG.Tweening;

namespace Catsland.Scripts.Bullets {
  [RequireComponent(typeof(ParticleSystem))]
  public class Explosion: MonoBehaviour {

    public float beforeExplosionSecond = 1.0f;
    public float afterExplosionSecond = 2.0f;
    public float radius = 1.0f;
    public int damage = 1;
    public float repel = 100.0f;

    // References
    private ParticleSystem particle;
    private CinemachineImpulseSource cinemachineImpulseSource;
    private CharacterEventSounds sounds;

    void Awake() {
      particle = GetComponent<ParticleSystem>();
      cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
      sounds = GetComponent<CharacterEventSounds>();
    }

    public void StartTimer() {
      particle.Emit(30);
      StartCoroutine(waitAndExplode());
    }

    private IEnumerator waitAndExplode() {
      yield return new WaitForSeconds(beforeExplosionSecond);
      explode();
    }

    private void explode() {
      foreach(Collider2D collider in Physics2D.OverlapCircleAll(transform.position, radius)) {
        collider.gameObject.SendMessage(
          MessageNames.DAMAGE_FUNCTION,
          new DamageInfo(damage, collider.bounds.center, collider.transform.position - transform.position, repel),
          SendMessageOptions.DontRequireReceiver);
      }

      cinemachineImpulseSource.GenerateImpulse();

      sounds?.PlayOnDamageSound();

      // Remove relay
      RelayPoint relay = GetComponentInChildren<RelayPoint>();
      if(relay != null) {
        Destroy(relay.gameObject);
      }

      StartCoroutine(waitAndDestroy());
    }

    private IEnumerator waitAndDestroy() {
      yield return new WaitForSeconds(afterExplosionSecond);
      Destroy(gameObject);
    }
  }
}

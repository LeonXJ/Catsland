using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller {
  [RequireComponent(typeof(PlayerController), typeof(Animator))]
  public class DrawAnimationV2 : MonoBehaviour {

    public SpriteRenderer spriteRenderer;
    public string shootingStateName = "UpperBodyShoot";

    public Sprite[] shootingSprite;

    private PlayerController playerController;
    private Animator animator;
    private float previousDrawIntensity = 0f;

    [Header("Effect")]
    public ParticleSystem drawContinousParticle;

    public float startStrongDrawParticleIntensity = .2f;
    public ParticleSystem strongDrawParticle;

    public float fullDrawIntesity = .9f;

    public float strongDrawReadyParticleIntensity = .9f;
    public ParticleSystem fullDrawParticle;
    public ParticleSystem fullDrawContinousParticle;

    private ParticleSystem.Particle[] fullDrawContinousParticleParticles;

    // Start is called before the first frame update
    void Start() {
      playerController = GetComponent<PlayerController>();
      animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {

    }

    void LateUpdate() {
      AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(2);
      bool playContinousParticle = false;
      if (stateInfo.IsName(shootingStateName)) {
        float intensity = playerController.getDrawIntensity();
        int index = (int)Mathf.Ceil(Mathf.Clamp(intensity, 0.0f, 1.0f) * (shootingSprite.Length - 1));
        spriteRenderer.sprite = shootingSprite[index];

        // particle
        if (!drawContinousParticle.isPlaying) {
          drawContinousParticle.Play(true);
        }

        if (playerController.canStrongShoot &&
          previousDrawIntensity < startStrongDrawParticleIntensity && intensity > startStrongDrawParticleIntensity) {
          drawContinousParticle.Emit(30);
          strongDrawParticle?.Play(true);
        }

        if (playerController.canStrongShoot &&
          previousDrawIntensity < strongDrawReadyParticleIntensity && intensity > strongDrawReadyParticleIntensity) {
          fullDrawParticle?.Play(true);
          fullDrawContinousParticle?.Play(true);
        }

        if (playerController.canStrongShoot && 
          intensity > fullDrawIntesity) {
          playContinousParticle = true;
        }

        previousDrawIntensity = intensity;
      } else {
        previousDrawIntensity = 0f;
        if (!drawContinousParticle.isStopped) {
          drawContinousParticle.Stop(true);
        }
      }

      if (playContinousParticle) {
        if (fullDrawContinousParticle.isStopped) {
          fullDrawContinousParticle.Play(true);
        }
      } else {
        if (fullDrawContinousParticle.isPlaying) {
          fullDrawContinousParticle.Stop();
          killAllParticlesOnFullDrawContinous();
        }
      }

    }

    private void killAllParticlesOnFullDrawContinous() {
      if (fullDrawContinousParticleParticles == null
        || fullDrawContinousParticleParticles.Length < fullDrawContinousParticle.main.maxParticles) {
        fullDrawContinousParticleParticles = new ParticleSystem.Particle[fullDrawContinousParticle.main.maxParticles];
      }
      int aliveParticleNum = fullDrawContinousParticle.GetParticles(fullDrawContinousParticleParticles);
      for (int i = 0; i < aliveParticleNum; i++) {
        ParticleSystem.Particle particle = fullDrawContinousParticleParticles[i];
        particle.remainingLifetime = 0f;
      }
    }
  }
}

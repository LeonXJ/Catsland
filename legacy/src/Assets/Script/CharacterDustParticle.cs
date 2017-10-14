using UnityEngine;
using System;
using Catslandx.Script.Sensor;

namespace Catslandx {
  /**
   * A making trailing dust component.
   * 
   * The component emits dust particles in certain cycle when the character:
   * - is moving on the ground with a horizontal speed greater then minGenerateDushSpeed.
   * - lands on the ground.
   * 
   * Input:
   * - PreCalculateSensor, to detect whether the character is grounded and the landing moment.
   * - Rigidbody2D, to get the character's horizontal speed.
   */ 
  [RequireComponent(typeof(ParticleSystem))]
  public class CharacterDustParticle :MonoBehaviour, IPreCalculateSensorSubscriber {

    private const float defaultGenerateDustMinSpeed = 0.01f;
    private const float defaultGenerateDushCycleInS = 0.5f;

    public PreCalculateSensor groundSensor;
    public Rigidbody2D characterRigidbody;
    
    // The threshold of characterRigidbody's mininal horizontal speed that can make dust.
    public float minGenerateDustSpeed = defaultGenerateDustMinSpeed;

    // The cycle of dust emit.
    public float generateDustCycleInS = defaultGenerateDushCycleInS;

    private ParticleSystem groundParticleSystem;
    private float currentCyclePhase = 0.0f;

    // Use this for initialization
    void Start() {
      if(groundSensor != null) {
        groundSensor.addSubscriber(this);
      }
      groundParticleSystem = GetComponent<ParticleSystem>();
      resetCyclePhase();
    }

    /*
     * Update the cycle.
     */ 
    void Update() {
      if(Math.Abs(characterRigidbody.velocity.x) > minGenerateDustSpeed) {
        currentCyclePhase -= Time.deltaTime;
        if(currentCyclePhase < 0.0f) {
          groundParticleSystem.Play();
          currentCyclePhase += generateDustCycleInS;
        }
      } else {
        resetCyclePhase();
      }
    }

    /*
     * Sets the emission attributes and emits particle on landing moment.
     * 
     * Triggered by ground pre-calculate sensor. Setting particleSystem.emission.enable means
     * the particle is allowed to emit. particleSystem.Play() emits the particles.
     */ 
    public void doUpdate(IPreCalculateSensor sensor) {
      GameObject groundGO = sensor.getCollideGO();
      ParticleSystem.EmissionModule emission = groundParticleSystem.emission;
      if(groundGO != null) {
        GroundMaterial groundMaterial = groundGO.GetComponent<GroundMaterial>();
        if(groundMaterial != null) {
          groundParticleSystem.startColor = groundMaterial.dustColor;
          emission.rate = new ParticleSystem.MinMaxCurve(groundMaterial.dustRate);
          emission.SetBursts(new ParticleSystem.Burst[] {
              new ParticleSystem.Burst(0.0f, groundMaterial.minDustBurst, groundMaterial.maxDustBurst)
          });
          emission.enabled = true;
          // Emit particle on landing.
          if(sensor.isOnTriggerOn()) {
            resetCyclePhase();
            groundParticleSystem.Play();
          }
          return;
        }
      }
      emission.enabled = false;
    }

    private void resetCyclePhase() {
      currentCyclePhase = generateDustCycleInS;
    }
  }
}

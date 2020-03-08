using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Catsland.Scripts.Common;
using Catsland.Scripts.Sound;

namespace Catsland.Scripts.Controller {
  public class MovingDoorController : MonoBehaviour {

    private const string IS_OPEN = "IsOpen";

    public bool enablePlayerSensor = true;
    public ParticleSystem ceilingDoorstepParticle;
    public ParticleSystem floorDoorstepParticle;

    public Sound.Sound doorStunSound;

    private CinemachineImpulseSource impluseSource;
    private Animator animator;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start() {
      impluseSource = GetComponent<CinemachineImpulseSource>();
      animator = GetComponent<Animator>();
      audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void Open() {
      animator.SetBool(IS_OPEN, true);
    }

    public void Close() {
      animator.SetBool(IS_OPEN, false);
    }

    public void PlayStun() {
      doorStunSound?.Play(audioSource);
    }

    void OnTriggerEnter2D(Collider2D collider) {
      if (enablePlayerSensor && collider.gameObject == SceneConfig.getSceneConfig().GetPlayer()) {
        Open();
      }
    }

    void OnTriggerExit2D(Collider2D collider) {
      if (enablePlayerSensor && collider.gameObject == SceneConfig.getSceneConfig().GetPlayer()) {
        Close();
      }
    }

    void OnDoorOpenComplete() {
      if (ceilingDoorstepParticle != null) {
        ceilingDoorstepParticle.Play();
      }
      if (impluseSource != null) {
        impluseSource.GenerateImpulse();
      }
      PlayStun();
    }

    void OnDoorCloseComplete() {
      if (floorDoorstepParticle != null) {
        floorDoorstepParticle.Play();
      }
      if (impluseSource != null) {
        impluseSource.GenerateImpulse();
      }
      PlayStun();
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class MovingDoorController : MonoBehaviour {

    private const string IS_OPEN = "IsOpen";

    public bool enablePlayerSensor = true;
    public ParticleSystem ceilingDoorstepParticle;
    public ParticleSystem floorDoorstepParticle;

    private CinemachineImpulseSource impluseSource;
    private Animator animator;

    // Start is called before the first frame update
    void Start() {
      impluseSource = GetComponent<CinemachineImpulseSource>();
      animator = GetComponent<Animator>();
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
    }

    void OnDoorCloseComplete() {
      if (floorDoorstepParticle != null) {
        floorDoorstepParticle.Play();
      }
      if (impluseSource != null) {
        impluseSource.GenerateImpulse();
      }
    }
  }
}

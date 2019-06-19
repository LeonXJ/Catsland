using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Catsland.Scripts.Controller {
  public class BattleGateController : MonoBehaviour {

    private CinemachineImpulseSource cinemachineImpulseSource;

    void Awake() {
      cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Shake() {
      if (cinemachineImpulseSource != null) {
        cinemachineImpulseSource.GenerateImpulse();
      }
    }

  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Catsland.Scripts.Controller.Test {
  public class ControllerVibrationTest : MonoBehaviour {


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void OnShoot() {
      Gamepad.current.SetMotorSpeeds(0.5f, 0.8f);
    }

  }
}

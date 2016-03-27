using UnityEngine;
using System.Collections;

namespace Catslandx {
  [RequireComponent(typeof(CharacterController2D))]
  public class UserController :MonoBehaviour {
    private CharacterController2D characterController;
    private bool jump = false;
    private bool dash = false;
    // Use this for initialization
    void Start() {
      characterController = GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update() {
      if(!jump) {
        jump = Input.GetKeyDown(KeyCode.J);
      }
      if(!dash) {
        dash = Input.GetKeyDown(KeyCode.K);
      }
    }

    private void FixedUpdate() {
      float h = Input.GetAxis("Horizontal");
      bool crouch = Input.GetAxis("Vertical") < -0.01f;
      
      characterController.move(h, jump, dash, crouch);
      
      jump = false;
      dash = false;
    }
  }
}
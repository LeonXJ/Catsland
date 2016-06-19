using UnityEngine;
using System.Collections;

namespace Catslandx {
  [RequireComponent(typeof(ICharacterController2D))]
  public class UserController :MonoBehaviour {
    private ICharacterController2D characterController;
    private bool jump = false;
    private bool dash = false;
    // Use this for initialization
    void Start() {
      characterController = GetComponent<ICharacterController2D>();
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
	  float v = Input.GetAxis ("Vertical");
      bool crouch = v < -0.01f;
      
	  characterController.move(new Vector2(h, v), jump, dash, crouch);
      
      jump = false;
      dash = false;
    }
  }
}
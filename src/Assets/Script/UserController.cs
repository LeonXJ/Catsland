using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class UserController :MonoBehaviour {
  private CharacterController characterController;
  private bool jump = false;
  private bool dash = false;
  // Use this for initialization
  void Start() {
    characterController = GetComponent<CharacterController>();
  }

  // Update is called once per frame
  void Update() {
    if (!jump) {
      jump = Input.GetKeyDown(KeyCode.J);
    }
    if (!dash) {
      dash = Input.GetKeyDown(KeyCode.K);
    }
  }

  private void FixedUpdate() {
    float h = Input.GetAxis("Horizontal");
    characterController.Move(Vector2.right * h, jump, dash);
    jump = false;
    dash = false;
  }
}

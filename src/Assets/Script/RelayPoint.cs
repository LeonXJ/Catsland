using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class RelayPoint :MonoBehaviour {

  private Collider collider;
  private SpriteRenderer spriteRender;

  private static Color ENABLE_COLOR = Color.red;
  private static Color DISABLE_COLOR = Color.white;
  // Use this for initialization
  void Start() {
    collider = GetComponent<Collider>();
    spriteRender = GetComponent<SpriteRenderer>();
  }

  // Update is called once per frame
  void Update() {

  }

  void OnTriggerEnter(Collider other) {
    /*
    CharacterController characterController = other.GetComponent<CharacterController>();
    if (characterController != null && characterController.supportRelay) {
      if (characterController.setRelay(this)) {
        if (spriteRender != null) {
          spriteRender.color = ENABLE_COLOR;
        }
      }
    }
    */
  }

  void OnTriggerExit(Collider other) {
    /*
    CharacterController characterController = other.GetComponent<CharacterController>();
    if(characterController != null && characterController.supportRelay) {
      characterController.cancelRelay(this);
      if (spriteRender != null) {
        spriteRender.color = DISABLE_COLOR;
      }
    }
    */
  }
}

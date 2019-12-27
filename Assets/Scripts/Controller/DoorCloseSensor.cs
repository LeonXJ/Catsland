using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class DoorCloseSensor : MonoBehaviour {

    public MovingDoorController doorController;

    public bool shouldCloseDoor = true;
    public bool shouldDisableControl = true;

    private void OnTriggerEnter2D(Collider2D collider) {
      if (collider.gameObject.tag != Tags.PLAYER) {
        return;
      }
      if (shouldCloseDoor) {
        doorController.Close();
      }
      if (shouldDisableControl) {
        doorController.enablePlayerSensor = false;
      }
    }
  }
}

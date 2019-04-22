using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Misc {
  public class Diamond : MonoBehaviour {

    public int value = 1;
    private bool collected = false;


    private void OnCollisionEnter2D(Collision2D collision) {
      if (collected) {
        return;
      }
      if (collision.gameObject.CompareTag(Tags.PLAYER)) {
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        Debug.Assert(playerController != null, "Player GameObject must have PlayerController component.");

        playerController.score += value;
        collected = true;

        Destroy(gameObject);
      }
    }
  }
}

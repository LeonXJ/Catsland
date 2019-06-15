using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Misc {
  public class Diamond : MonoBehaviour {

    public int value = 1;
    private bool collected = false;


    private void OnTriggerEnter2D(Collider2D collision) {
      onHit(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      onHit(collision.gameObject);
    }

    private void onHit(GameObject other) {
      if (collected) {
        return;
      }
      if (other.CompareTag(Tags.PLAYER)) {
        PlayerController playerController = other.GetComponent<PlayerController>();
        playerController.score += value;
        collected = true;

        Destroy(gameObject);
      }
    }
  }
}

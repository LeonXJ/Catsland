using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Misc {
  public class Diamond : MonoBehaviour {

    public int value = 1;
    public Sound.Sound diamondCollect;
    public Sound.Sound diamondHitGround;
    private bool collected = false;

    private Collider2D collider;

    void Start() {
      collider = GetComponent<Collider2D>();
    }


    private void OnTriggerEnter2D(Collider2D collider2d) {
      onHit(collider2d);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      onHit(collision.collider);
    }

    private void onHit(Collider2D other) {
      if (collected) {
        return;
      }
      if (other.gameObject.CompareTag(Tags.PLAYER)) {
        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
        playerController.score += value;
        collected = true;
        diamondCollect?.PlayOneShot(transform.position);
        Destroy(gameObject);
        return;
      }
      if (other.gameObject.layer == Layers.LayerGround) {
        diamondHitGround?.PlayOneShot(transform.position);
      }
      // Other character will not collider with diamond
      if (other.gameObject.layer == Common.Layers.LayerCharacter) {
        Physics2D.IgnoreCollision(other, collider);
      
      } 
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class Fragment : MonoBehaviour {

    public GameObject ignoreGo;

    private Collider2D collider;

    private void Awake() {
      collider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      if (collision.gameObject == ignoreGo) {
        Physics2D.IgnoreCollision(collision.collider, collider);
      }
    }
  }
}

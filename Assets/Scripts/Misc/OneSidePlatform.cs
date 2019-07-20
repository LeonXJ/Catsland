using System.Collections;
using UnityEngine;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Misc {

  public class OneSidePlatform : MonoBehaviour {

    public float delayEnableInS = 1.0f;

    private Collider2D collider;

    private void Awake() {
      collider = GetComponent<Collider2D>();
    }

    void OnCollisionStay2D(Collision2D collision) {
      if (!collision.gameObject.CompareTag(Common.Tags.PLAYER)) {
        return;
      }

      Rigidbody2D rb2d = collision.gameObject.GetComponent<Rigidbody2D>();
      PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
      if (rb2d.velocity.y < 0.0f && playerController.footPosition.y < transform.position.y) {
        StartCoroutine(enableCollision(1.0f));
      }
    }

    private IEnumerator enableCollision(float delay) {
      collider.enabled = false;
      yield return new WaitForSeconds(delay);
      collider.enabled = true;
    }
  }
}

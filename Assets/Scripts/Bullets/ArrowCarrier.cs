using System.Collections;
using UnityEngine;

namespace Catsland.Scripts.Bullets {

  [RequireComponent(typeof(Rigidbody2D))]
  public class ArrowCarrier :MonoBehaviour {

    private Rigidbody2D rb2d;

    public void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
    }

    public IEnumerator fire(Vector2 direction, float lifetime) {
      // velocity and orientation
      rb2d.velocity = direction;
      transform.localScale = new Vector2(
        direction.x > 0.0f
            ? Mathf.Abs(transform.localScale.x)
            : -Mathf.Abs(transform.localScale.x),
        1.0f);

      // self destory
      yield return new WaitForSeconds(lifetime);
      Destroy(gameObject);
    }
  }
}

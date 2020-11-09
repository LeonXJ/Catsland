using UnityEngine;

namespace Catsland.Scripts.Bullets {
  public class BulletUtils {

    public static GameObject generateDebrid(GameObject prefab, Transform transform, Vector2 velocity, float spinSpeed, float drag = .3f) {
      GameObject brokenArrow = GameObject.Instantiate(prefab);
      brokenArrow.transform.position = transform.position;
      brokenArrow.transform.localScale = transform.lossyScale;

      // assign random velocity
      Rigidbody2D[] brokenParts = brokenArrow.GetComponentsInChildren<Rigidbody2D>();
      foreach(Rigidbody2D part in brokenParts) {
        part.angularVelocity = spinSpeed;
        part.velocity = velocity;
        part.drag = drag;
      }

      return brokenArrow;
    }
  }
}

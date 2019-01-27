using UnityEngine;

namespace Catsland.Scripts.Bullets {
  public class BulletUtils {

    public static GameObject generateDebrid(GameObject prefab, Transform transform, float horizontalV, float spinSpeed) {
      GameObject brokenArrow = GameObject.Instantiate(prefab);
      brokenArrow.transform.position = transform.position;
      brokenArrow.transform.localScale = transform.lossyScale;

      // assign random velocity
      Rigidbody2D[] brokenParts = brokenArrow.GetComponentsInChildren<Rigidbody2D>();
      foreach(Rigidbody2D part in brokenParts) {
        part.angularVelocity = spinSpeed;
        part.velocity = new Vector2(horizontalV, 0.0f);
      }

      return brokenArrow;
    }
  }
}

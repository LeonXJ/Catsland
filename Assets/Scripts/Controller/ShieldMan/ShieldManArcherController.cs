using UnityEngine;
using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.Controller.ShieldMan {

  public class ShieldManArcherController: ShieldManController {

    [Header("Arrow")]
    public GameObject arrowPrefab;
    public Transform arrowShootPoint;
    public float arrowSpeed = 10f;
    public float arrowLifetime = 3f;
    public Party.WeaponPartyConfig weaponPartyConfig;

    public void Shoot() {
      GameObject arrow = Instantiate(arrowPrefab);
      arrow.transform.position = arrowShootPoint.position;

      Vector2 arrowVelocity = new Vector2(arrowSpeed * getOrientation(), 0f);

      ArrowCarrier arrowCarrier = arrow.GetComponent<ArrowCarrier>();
      arrowCarrier.fire(arrowVelocity, arrowLifetime, gameObject.tag, weaponPartyConfig);
    }
  }
}

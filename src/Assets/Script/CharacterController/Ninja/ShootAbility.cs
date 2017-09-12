using UnityEngine;
using Catslandx.Script.CharacterController.Common;

namespace Catslandx.Script.CharacterController.Ninja {
public class ShootAbility : AbstractCharacterAbility {

    public enum ShootSubstatus {
      STANDBY,
      PREPARE,
      PERFORM,
    }

    public float shootPrepareInMs;
    public float shootPerformInMs;
    public Vector2 positionOffset;
    public float arrowSpeed = 1.0f;
    public GameObject bulletPrototype;

    private ICharacterController characterController;

    private void Awake() {
      characterController = GetComponent<ICharacterController>();
    }

    public GameObject createBulletGO() {
      Vector2 position = MathHelper.applyOffset(
        transform.position,
        characterController.transformRightOrientationVectorToCurrentOrientation(positionOffset));
      GameObject bulletObject = GameObject.Instantiate(bulletPrototype);
      bulletObject.transform.position = position;

      Rigidbody2D rigidbody = bulletObject.GetComponent<Rigidbody2D>();
      if(rigidbody != null) {
        rigidbody.velocity = characterController.transformRightOrientationVectorToCurrentOrientation(
          new Vector2(arrowSpeed, 0.0f));
      }

      BulletHead bulletHead = bulletObject.GetComponent<BulletHead>();
      if(bulletHead != null) {
        bulletHead.fire(gameObject);
      }
      return bulletObject;
    }

    public float getSubstatusTimeInMs(ShootSubstatus shootSubstatus) {
      switch(shootSubstatus) {
        case ShootSubstatus.PREPARE:
          return shootPrepareInMs;
        case ShootSubstatus.PERFORM:
          return shootPerformInMs;
      }
      return 0.0f;
    }
}
}

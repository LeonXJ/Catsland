using UnityEngine;
using Catslandx.Script.CharacterController.Common;

namespace Catslandx.Script.CharacterController.Ninja {

  [RequireComponent(typeof(ICharacterController))]
  public class MeleeAbility :AbstractCharacterAbility {

    public enum AttackSubstatus {
      STANDBY,
      PREPARE,
      PERFORM,
    }

    public float meleePrepreInMs;
    public float meleePerformInMs;
    public Vector2 positionOffset;
    public GameObject meleePrototype;

    private ICharacterController characterController;

    void Awake() {
      characterController = GetComponent<ICharacterController>();
    }

    public GameObject createMeleeGO() {
      Vector2 position = MathHelper.applyOffset(
        transform.position,
        characterController.transformRightOrientationVectorToCurrentOrientation(positionOffset));
      GameObject meleeObject = GameObject.Instantiate(meleePrototype, transform);
      meleeObject.transform.position = position;

      BulletHead bulletHead = meleeObject.GetComponent<BulletHead>();
      if(bulletHead != null) {
        bulletHead.fire(gameObject);
      }
      return meleeObject;
    }

    public float getSubstatusTimeInMs(AttackSubstatus substatus) {
      switch(substatus) {
        case AttackSubstatus.PREPARE:
          return meleePrepreInMs;
        case AttackSubstatus.PERFORM:
          return meleePerformInMs;
      }
      return 0.0f;
    }
  }
}

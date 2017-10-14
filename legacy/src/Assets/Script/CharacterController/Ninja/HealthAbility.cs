using System;
using Catslandx.Script.CharacterController.Common;
using Catslandx.Script.Common;
using UnityEngine;

namespace Catslandx.Script.CharacterController.Ninja {
  
  /** The charater can be damaged with this ability. */
  [RequireComponent(typeof(ICharacterController))]
  [RequireComponent(typeof(Rigidbody2D))]
  public class HealthAbility :AbstractCharacterAbility, IVulnerable {

    public float freezeInMs = 1.0f;
    public float repelFactor = 100.0f;

    private Rigidbody2D rigidbody2d;
    private ICharacterController chracterController;

    public void Awake() {
      rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
      chracterController = gameObject.GetComponent<ICharacterController>();
    }

    public bool getCanGetHurt() {
      return true;
    }

    public int getHurt(int hurtPoint, Vector2 repelForce) {
      rigidbody2d.AddForce(repelForce * repelFactor);
      chracterController.transitToStatus<HurtStatus>();
      return hurtPoint;
    }

    public void respawn() {
      throw new NotImplementedException();
    }
  }
}

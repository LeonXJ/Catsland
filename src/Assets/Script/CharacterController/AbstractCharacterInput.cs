using UnityEngine;
using System.Collections;
using System;

namespace Catslandx {
  public class AbstractCharacterInput :MonoBehaviour, ICharacterInput {

    protected Vector2 wantDirection;
    protected bool wantAttack;
    protected bool wantJump;
    protected bool wantDash;
    protected bool wantInteract;

    Vector2 ICharacterInput.wantDirection() {
      throw new NotImplementedException();
    }

    bool ICharacterInput.wantAttack() {
      throw new NotImplementedException();
    }

    bool ICharacterInput.wantJump() {
      throw new NotImplementedException();
    }

    bool ICharacterInput.wantDash() {
      throw new NotImplementedException();
    }

    bool ICharacterInput.wantInteract() {
      throw new NotImplementedException();
    }
  }
}

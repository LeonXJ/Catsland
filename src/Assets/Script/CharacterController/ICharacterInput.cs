using UnityEngine;
using System.Collections;

namespace Catslandx {
  public interface ICharacterInput {

    Vector2 wantDirection();
    bool wantAttack();
    bool wantJump();
    bool wantDash();
    bool wantInteract();

  }
}

using UnityEngine;

namespace Catslandx.Script.Input {

  /** Instruction input of character. */
  public interface ICharacterInput {

    /** Gets and stores input status. */ 
    void updateInput(float deltaTime);

    Vector2 wantDirection();
    bool wantAttack();
    bool wantJump();
    bool wantDash();
    bool wantInteract();
  }
}

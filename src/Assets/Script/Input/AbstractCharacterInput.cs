using UnityEngine;

namespace Catslandx.Script.Input {

  /** Abstract implementation of ICharacterInput. */
  public abstract class AbstractCharacterInput :MonoBehaviour, ICharacterInput {

    protected Vector2 wantDirection;
    protected bool wantAttack;
    protected bool wantJump;
    protected bool wantDash;
    protected bool wantInteract;

    Vector2 ICharacterInput.wantDirection() {
      return wantDirection;
    }

    bool ICharacterInput.wantAttack() {
      return wantAttack;
    }

    bool ICharacterInput.wantJump() {
      return wantJump;
    }

    bool ICharacterInput.wantDash() {
      return wantDash;
    }

    bool ICharacterInput.wantInteract() {
      return wantInteract;
    }

    public abstract void updateInput(float deltaTime);
  }
}

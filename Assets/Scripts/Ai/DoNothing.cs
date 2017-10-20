using UnityEngine;

using Catsland.Scripts.CharacterController;

namespace Catsland.Scripts.Ai {
  public class DoNothing :MonoBehaviour, IInput {

    public bool attack() {
      return false;
    }

    public float getHorizontal() {
      return 0.0f;
    }

    public float getVertical() {
      return 0.0f;
    }

    public bool jump() {
      return false;
    }
  }
}

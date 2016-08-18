using UnityEngine;
using System.Collections;

namespace Catslandx {
  public interface ICharacterController {

    IState transitToState();

    IState innerTransteToState();

    IState getCurrentState();

    IState reset();
  }
}

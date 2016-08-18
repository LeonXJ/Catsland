using UnityEngine;
using System.Collections;
using System;

namespace Catslandx {
  public class AbstractCharacterController :MonoBehaviour, ICharacterController {

    IState ICharacterController.getCurrentState() {
      throw new NotImplementedException();
    }

    IState ICharacterController.innerTransteToState() {
      throw new NotImplementedException();
    }

    IState ICharacterController.reset() {
      throw new NotImplementedException();
    }

    // Use this for initialization
    void Start() {

    }

    IState ICharacterController.transitToState() {
      throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update() {

    }

  }
}

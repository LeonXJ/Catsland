using UnityEngine;
using System.Collections.Generic;

namespace Catslandx {
  public abstract class AbstractState: IState {

    private GameObject gameObject;

    public AbstractState(GameObject gameObject) {
      this.gameObject = gameObject;
    }

    protected GameObject getGameObject() {
      return gameObject;
    }

    protected T getComponent<T>() {
      return gameObject.GetComponent<T>();
    }

    public abstract void onEnter(IState previousState);

    public abstract IState update(Dictionary<SensorEnum, ISensor> sensors, ICharacterInput input, float deltaTime);

    public abstract void onExit(IState nextState);

  }
}

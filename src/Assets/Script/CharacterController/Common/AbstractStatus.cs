using System.Collections.Generic;
using UnityEngine;
using Catslandx.Script.Sensor;
using Catslandx.Script.Input;

namespace Catslandx.Script.CharacterController.Common {

  /** Abstract implementation of IStatus. */
  public abstract class AbstractStatus :IStatus {

    private GameObject gameObject;
    private StatusFactory stateFactory;

    public AbstractStatus(
        GameObject gameObject, StatusFactory stateFactory) {
      this.gameObject = gameObject;
      this.stateFactory = stateFactory;
    }

    protected GameObject getGameObject() {
      return gameObject;
    }

    public StatusFactory getStateFactory() {
      return stateFactory;
    }

    /** Get component from current GameObject. */
    protected T getComponent<T>() {
      return gameObject.GetComponent<T>();
    }

    public virtual void onEnter(IStatus previousStatus) {}

    public virtual IStatus update(
        Dictionary<SensorEnum, ISensor> sensors,
        ICharacterInput input,
        float deltaTime) {
      return this;
    }

    public virtual void onExit(IStatus nextStatus) {}
  }
}

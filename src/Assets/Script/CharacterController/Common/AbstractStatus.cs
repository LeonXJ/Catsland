using System.Collections.Generic;
using UnityEngine;
using Catslandx.Script.Sensor;
using Catslandx.Script.Input;

namespace Catslandx.Script.CharacterController.Common {

  /** Abstract implementation of IStatus. */
  public abstract class AbstractStatus :IStatus {

    private GameObject gameObject;
    private StatusFactory stateFactory;
    protected ICharacterController characterController;

    public AbstractStatus(
        GameObject gameObject, StatusFactory stateFactory) {
      this.gameObject = gameObject;
      this.stateFactory = stateFactory;
      this.characterController = getComponent<ICharacterController>();
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

    public virtual bool isEligible() {
      return true;
    }

    public virtual void onEnter(IStatus previousStatus) {}

    public virtual IStatus update(
        Dictionary<SensorEnum, ISensor> sensors,
        ICharacterInput input,
        float deltaTime) {
      return this;
    }

	protected ISensor getSensorOrNull(Dictionary<SensorEnum, ISensor> sensors, SensorEnum sensorEnum) {
	  return Dictionaries<SensorEnum, ISensor>.getOrDefault(sensors, sensorEnum, null);
	}

    public virtual void onExit(IStatus nextStatus) {}

    public virtual void applyAnimation(Animator animator) {}
  }
}

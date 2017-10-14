using Catslandx.Script.Input;
using Catslandx.Script.Sensor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Catslandx.Script.CharacterController.Common {

  /** Abstract implementation of CharacterController. */
  public abstract class AbstractCharacterController :MonoBehaviour, ICharacterController {


    // Senseros
    protected Dictionary<SensorEnum, ISensor> sensors =
        new Dictionary<SensorEnum, ISensor>();

    // Inputs
    protected ICharacterInput characterInput;

    // Status
    protected StatusFactory stateFactory;
    protected IStatus currentStatus;

    // Additional info
    private Orientation orientation;

    // Initializations

      /** Calls serial of initializations.
       *
       * Called by Unity. 
       */
    void Start() {
      initializeSensor();
      initializeInput();
      stateFactory = new StatusFactory(gameObject);
      currentStatus = getInitialStatus();
    }

    /** Gets the initial status. */
    protected abstract IStatus getInitialStatus();

    /** Initializes the sensors.
     *
     * Implementation should get the sensor instance and put them into sensors.
     */
    protected virtual void initializeSensor() { }

    /** Initializes the input. */
    protected virtual void initializeInput() { 
      characterInput = GetComponent<ICharacterInput>();
    }

    // Updates

    /** Triggers the serials of updating functions.
     * 
     * Called by Unity.
     */ 
    void Update() {
      float deltaTime = Time.deltaTime;
      updateSensor(deltaTime);
      updateInput(deltaTime);
      updateMovement(deltaTime);
      updateAnimation(deltaTime);
    }

    /** Updates sensors. */
    protected virtual void updateSensor(float deltaTime) {
      foreach(ISensor sensor in sensors.Values) {
        if(sensor is IPreCalculateSensor) {
          (sensor as IPreCalculateSensor).preCalculate();
        }
      }
    }

    /** Updates inputs. */
    protected virtual void updateInput(float deltaTime) {
	  if(characterInput != null) {
		characterInput.updateInput(deltaTime);
	  }
    }

    /** Updates the character movement. */
    protected virtual void updateMovement(float deltaTime) {
      IStatus nextState = currentStatus.update(sensors, characterInput, deltaTime);
      if (nextState != currentStatus) {
        currentStatus.onExit(nextState);
        nextState.onEnter(currentStatus);
        currentStatus = nextState;
      }
    }

    /** Update the animation. */
    protected virtual void updateAnimation(float deltaTime) {}

    public void setOrientation(Orientation orientation) {
      this.orientation = orientation;
    }

    public Orientation getOrientation() {
      return orientation;
    }

    public Vector2 transformRightOrientationVectorToCurrentOrientation(Vector2 rightOrientationVector) {
      return orientation == Orientation.Right ? rightOrientationVector : new Vector2(-rightOrientationVector.x, rightOrientationVector.y);
    }

    IStatus ICharacterController.getCurrentState() {
      throw new NotImplementedException();
    }

    IStatus ICharacterController.innerTransteToState() {
      throw new NotImplementedException();
    }

    IStatus ICharacterController.reset() {
      throw new NotImplementedException();
    }

    public IStatus transitToStatus<T> () where T : IStatus {
      IStatus newStatus = stateFactory.getState<T>();
      if(currentStatus != newStatus) {
        currentStatus.onExit(newStatus);
        newStatus.onEnter(currentStatus);
        currentStatus = newStatus;
      }
      return currentStatus;
    }
  }
}

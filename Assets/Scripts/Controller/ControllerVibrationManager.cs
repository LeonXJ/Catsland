using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Catsland.Scripts.Controller {
  public class ControllerVibrationManager : MonoBehaviour {

    class VibrationState {
      public ControllerVibrationStackEffectConfig config;
      public float registerTime;
    }

    private LinkedList<VibrationState> vibrationStates = new LinkedList<VibrationState>();

    private void Awake() {
      DontDestroyOnLoad(gameObject);
    }

    // Register config and return whether the config is actually registered.
    public bool RegisterConfig(ControllerVibrationStackEffectConfig config, bool overwrite = false) {
      if (config.deduplicate) {
        foreach (VibrationState state in vibrationStates) {
          if (state.config.name == config.name) {
            if (overwrite) {
              state.config = config;
              state.registerTime = Time.time;
              return true;
            }
            // Not overwrite existing config.
            return false;
          }
        }
      }

      // Not dedup or no existing same name config.
      vibrationStates.AddLast(new VibrationState { config = config, registerTime = Time.time });
      return true;
    }

    public void UnregisterConfig(ControllerVibrationStackEffectConfig config) {
      LinkedListNode<VibrationState> node = vibrationStates.First;
      while (node != null) {
        if (node.Value.config.name == config.name) {
          var toBeRemoved = node;
          node = node.Next;
          vibrationStates.Remove(toBeRemoved);
          continue;
        }
        node = node.Next;
      }
    }

    // Update is called once per frame
    void Update() {
      float lowFrequency = 0f;
      float highFrequency = 0f;
      LinkedListNode<VibrationState> node = vibrationStates.First;
      while (node != null) {
        VibrationState state = node.Value;
        if (state.registerTime + state.config.lastSeconds < Time.time) {
          // should be removed
          var toBeRemoved = node;
          node = node.Next;
          vibrationStates.Remove(toBeRemoved);
          continue;
        }
        // effective state.
        lowFrequency = Mathf.Max(lowFrequency, state.config.lowFrequency);
        highFrequency = Mathf.Max(highFrequency, state.config.highFrequency);
        node = node.Next;
      }
      Gamepad.current.SetMotorSpeeds(lowFrequency, highFrequency);
    }
  }
}

using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

namespace Catslandx.Script.CharacterController.Common {

  /** The factory to generate and cache status object. */
  public class StatusFactory {

    private readonly GameObject gameObject;

    // Map Type of status class to the instance.
    private readonly Dictionary<Type, IStatus> states = new Dictionary<Type, IStatus>();

    public StatusFactory(GameObject gameObject) {
      this.gameObject = gameObject;
    }

    /** Gets (creats if not exists) the status instance of given type T. */
    public IStatus getState<T>() where T : IStatus {
      Type type = typeof(T);
      if(!states.ContainsKey(type)) {
        IStatus state = createState<T>();
        states.Add(type, state);
      }
      return states[type];
    }

    private IStatus createState<T>() where T : IStatus {
      ConstructorInfo constructor = typeof(T).GetConstructor(
        new Type[] { typeof(GameObject), typeof(StatusFactory) });
      return constructor.Invoke(new object[] { gameObject, this }) as IStatus;
    }
  }
}

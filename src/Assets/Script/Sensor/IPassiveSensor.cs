using UnityEngine;

// TODO: deprecated?
namespace Catslandx.Script.Sensor {

  /** A kind of sensor that only does detection when being queried. */
  public interface IPassiveSensor :ISensor {

    void notifyEnter(GameObject gameObject);

    void notifyExit(GameObject gameObject);
  }
}

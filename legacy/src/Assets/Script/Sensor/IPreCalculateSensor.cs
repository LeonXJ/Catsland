using UnityEngine;

namespace Catslandx.Script.Sensor {

  /** A type of sensor which does detection at once.
   *
   * This type of sensor does detection and notify the subscriber in preCalculate().
   */
  public interface IPreCalculateSensor : ISensor {

    /** Add the subscriber to this sensor.
     * 
     * Subscriber will be notified in preCalculate() if the sensor is triggerred.
     */ 
    void addSubscriber(IPreCalculateSensorSubscriber subscriber);

    /** Calculate whether the sensor is triggerred. */
    void preCalculate();

    /** Get the object which triggers this sensor. */
    GameObject getCollideGO();
  }
}

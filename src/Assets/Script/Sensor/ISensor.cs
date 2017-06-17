namespace Catslandx.Script.Sensor {

  /** Sensor interface. */
  public interface ISensor {

    /** Returns whether the sensor goes from Off to On status. */
    bool isOnTriggerOn();

    /** Returns whether the sensor is in On status. */
    bool isInTrigger();

    /** Returns whether the sensor goes from On to Off status. */
    bool isOnTriggerOff();
  }
}

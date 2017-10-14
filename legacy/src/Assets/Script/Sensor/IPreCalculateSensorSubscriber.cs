namespace Catslandx.Script.Sensor {

  /** The subscriber of the PreCalculateSensor. */
  public interface IPreCalculateSensorSubscriber {

    /** Does updating according to sensor status. */
    void doUpdate(IPreCalculateSensor sensor);
  }
}

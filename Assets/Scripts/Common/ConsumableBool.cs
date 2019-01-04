namespace Catsland.Scripts.Common {
  public class ConsumableBool {
    private bool value = false;

    public void setTrue() {
      value = true;
    }

    public bool getAndReset() {
      if(value) {
        value = false;
        return true;
      }
      return false;
    }
  }
}

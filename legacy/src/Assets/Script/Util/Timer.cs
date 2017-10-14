using UnityEngine;

using Catslandx.Script.Exception;

namespace Catslandx.Util {
  public class Timer {

    public float cycle;

    private float phase;

    public void updateTimer(float deltaTimeInS) {
      if (cycle < Mathf.Epsilon) {
        throw new InvalidParameterException("Time.cycle cannot be equal or less than 0");
      }
      phase -= deltaTimeInS;
      while(phase < 0.0f) {

        phase += cycle;
      }
    }

  }
}

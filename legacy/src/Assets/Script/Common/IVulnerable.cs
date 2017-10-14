using UnityEngine;

namespace Catslandx.Script.Common {
  public interface IVulnerable {

    bool getCanGetHurt();
    int getHurt(int hurtPoint, Vector2 repelForce);

    // TODO: deprecate
    void respawn();
  }
}


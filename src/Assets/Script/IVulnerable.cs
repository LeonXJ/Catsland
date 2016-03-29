using UnityEngine;
using System.Collections;

namespace Catslandx {
  public interface IVulnerable {
    bool getCanGetHurt();
    int getHurt(int hurtPoint, Vector2 repelForce);
  }
}


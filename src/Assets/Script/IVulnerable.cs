using UnityEngine;
using System.Collections;

namespace Catslandx {
  public interface IVulnerable {
    bool canGetHurt();
    int getHurt(int hurtPoint, Vector2 repelForce);
  }
}


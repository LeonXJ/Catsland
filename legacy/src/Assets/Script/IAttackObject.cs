using UnityEngine;
using System.Collections;

namespace Catslandx {
  public interface IAttackObject {
    void init(GameObject owner, int attack);
    void activate();
  }
}

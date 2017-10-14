using UnityEngine;
using System.Collections;

namespace Catslandx {
  public interface IRespawn {
    void setRespawn(GameObject respawnPosition);
    void doRespawn();
  }
}

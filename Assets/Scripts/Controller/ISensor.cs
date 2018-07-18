using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller {
  public interface ISensor {
    bool isStay();
    HashSet<GameObject> getTriggerGos();
  }
}
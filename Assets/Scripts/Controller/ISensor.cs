using UnityEngine;

namespace Catsland.Scripts.Controller {
  public interface ISensor {
    bool isStay();
    GameObject getTriggerGO();
  }
}
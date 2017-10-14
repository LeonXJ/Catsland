using UnityEngine;
using System.Collections;

namespace Catslandx {
  public interface IEvent {
    void trigger(GameObject gameObject);
  }
}

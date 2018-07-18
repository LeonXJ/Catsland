using UnityEngine;
using System.Collections.Generic;

public class Utils {
  public static GameObject getAnyFrom(IEnumerable<GameObject> gameObjects) {
    foreach(GameObject gameObject in gameObjects) {
      return gameObject;
    }
    return null;
  }
}

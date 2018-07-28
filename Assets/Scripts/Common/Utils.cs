using UnityEngine;
using System.Collections.Generic;

public class Utils {
  public static GameObject getAnyFrom(IEnumerable<GameObject> gameObjects) {
    foreach(GameObject gameObject in gameObjects) {
      return gameObject;
    }
    return null;
  }

  public static void setRelativeRenderLayer(
    SpriteRenderer mainRenderer, SpriteRenderer subRenderer, int offset) {
    subRenderer.sortingOrder = mainRenderer.sortingOrder + offset;
  }
}

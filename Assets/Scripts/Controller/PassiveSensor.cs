using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  // NOT in used before editor is created for this object. 
  public class PassiveSensor: ISensor {

    public Color untriggeredColor = Color.gray;
    public Color triggeredColor = Color.red;
    public LayerMask layerMask;
    public Rect rect;

    private GameObject gameObject;

    public PassiveSensor(GameObject gameObject) {
      this.gameObject = gameObject;
    }


    public HashSet<GameObject> getTriggerGos() {
      return new HashSet<GameObject>((new List<Collider2D>(Physics2D.OverlapBoxAll(
        gameObject.transform.TransformPoint(rect.position),
        rect.size, 0.0f, layerMask))).ConvertAll(c => c.gameObject));
    }

    public bool isStay() {
      return Physics2D.OverlapBox(
        gameObject.transform.TransformPoint(rect.position),
        rect.size, 0.0f, layerMask) != null;
    }

    public void onDrawGizmosSelected() {
      Utils.drawRectAsGizmos(rect, isStay() ? triggeredColor : untriggeredColor, gameObject.transform);
    }
  }
}

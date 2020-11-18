using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Bullets {

  // Class for vulernable attributes.
  [System.Serializable]
  public class VulnerableAttribute {

    public int maxHealth = 2;
    public int currentHealth = 2;

    public float arrowHitRepelSpeed = 1f;

    public float knockbackRepelSpeed = 1.2f;

    public float strongArrowHitRepelSpeed = 1.2f;



  }
}

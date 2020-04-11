using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller.Slime {
  public class SlimeGenerator : MonoBehaviour {

    public void Generate(SlimeGenerationConfig config) {

      GameObject slimeSilhouetteGo = Instantiate(config.silhouettePrefab);
      slimeSilhouetteGo.transform.position = config.silhouetteInitPosition;
      float jumpUpSpeed = Random.Range(config.silhouetteJumpUpSpeedMin, config.silhouetteJumpUpSpeedMax);
      slimeSilhouetteGo.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, jumpUpSpeed);

      SlimeSilhouette slimeSilhouette = slimeSilhouetteGo.GetComponent<SlimeSilhouette>();
      slimeSilhouette.slimePrefab = config.slimePrefab;
      slimeSilhouette.slimeZ = config.slimeZ;
    }
  }
}

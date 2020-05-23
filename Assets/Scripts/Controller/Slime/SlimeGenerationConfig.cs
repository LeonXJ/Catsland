using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller.Slime {
  public class SlimeGenerationConfig : MonoBehaviour {

    public Transform canonicalSilhouetteInitPosition;
    public float silhouetteInitPositionHorizontalRandom = 0.2f;

    public float silhouetteJumpUpSpeedMin = 8f;
    public float silhouetteJumpUpSpeedMax = 9f;

    public GameObject silhouettePrefab;
    public GameObject slimePrefab;
    public float slimeZ = .1f;

    public KeyCode debugGenerationKeyCode = KeyCode.Alpha5;

    public Vector3 silhouetteInitPosition {
      get {
        Vector3 randomOffset = new Vector3(
          Random.Range(-silhouetteInitPositionHorizontalRandom, silhouetteInitPositionHorizontalRandom),
          0f,
          0f);
        if (canonicalSilhouetteInitPosition != null) {
          return canonicalSilhouetteInitPosition.position + randomOffset;
        }
        return transform.position + randomOffset;
      }
    }

    void Update() {
      /*
      if (Input.GetKeyDown(debugGenerationKeyCode)) {
        SlimeGenerator generator = FindObjectOfType<SlimeGenerator>();
        generator.Generate(this);
      }
      */
    }
  }
}

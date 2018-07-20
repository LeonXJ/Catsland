using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class SenseReact :MonoBehaviour {

    public Color fullSenseLight;

    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;

    void Awake() {
      playerController =
        GameObject.FindGameObjectWithTag(Tags.PLAYER).GetComponent<PlayerController>();
      spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start() {
      spriteRenderer.material.SetColor("_Color", Color.black);

    }

    void Update() {
      Color senseLight = Color.Lerp(Color.black, fullSenseLight, playerController.currentSense);
      spriteRenderer.material.SetColor("_AmbientLight", senseLight);
    }
  }

}

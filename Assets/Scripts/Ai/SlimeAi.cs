using UnityEngine;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ai {
  public class SlimeAi : MonoBehaviour, SlimeController.SlimeInput  {

    public float detectPlayerDistance = 8.0f;
    public float shieldGenerationChance = .2f;

    private float horizontal = 0.0f;
    private SlimeController slimeController;
    private bool generateShield;

    private Utils utils;

    private void Awake() {
      utils = new Utils(gameObject);
      slimeController = GetComponent<SlimeController>();
    }

    // Update is called once per frame
    void Update() {
      generateShield = slimeController.canGenerateShild() && Random.Range(0f, 1f) < shieldGenerationChance;

      Vector2 deltaToPlayer = utils.getDistanceToPlayer();
      if (deltaToPlayer.sqrMagnitude < detectPlayerDistance * detectPlayerDistance) {
        horizontal = Mathf.Sign(deltaToPlayer.x);
      }
    }

    public float getHorizontal() {
      return horizontal;
    }

    public bool wantGenerateShield() {
      return generateShield;
    }
  }
}

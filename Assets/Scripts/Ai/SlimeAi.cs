using UnityEngine;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ai {
  public class SlimeAi : MonoBehaviour, SlimeController.SlimeInput  {

    public float detectPlayerDistance = 8.0f;

    private float horizontal = 0.0f;

    private Utils utils;

    private void Awake() {
      utils = new Utils(gameObject);
    }

    // Update is called once per frame
    void Update() {
      Vector2 deltaToPlayer = utils.getDistanceToPlayer();
      if (deltaToPlayer.sqrMagnitude < detectPlayerDistance * detectPlayerDistance) {
        horizontal = Mathf.Sign(deltaToPlayer.x);
      }
    }

    public float getHorizontal() {
      return horizontal;
    }
  }
}

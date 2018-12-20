using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class RelayPoint: MonoBehaviour {

    public GameObject currentHintGo;
    public GameObject targetHintGo;
    public int hintCircleSegment = 16;

    private GameObject playerGo;
    private PlayerController playerController;
    private SpriteRenderer currentHintRenderer;
    private SpriteRenderer targetHintRenderer;

    void Awake() {
      playerGo = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      Debug.AssertFormat(playerGo != null, "Cannot get Player gameObject via tag: %s", Tags.PLAYER);

      playerController = playerGo.GetComponent<PlayerController>();
      Debug.AssertFormat(playerController != null, "Cannot get PlayerController within Player GameObject: " + playerGo.name);

      currentHintRenderer = currentHintGo.GetComponent<SpriteRenderer>();
      targetHintRenderer = targetHintGo.GetComponent<SpriteRenderer>();
    }

    void Update() {
      if(!playerController.supportRelay) {
        return;
      }
      Vector2 delta = playerController.relayDeterminePoint.position - gameObject.transform.position;
      float distance = delta.magnitude;

      // update controller info
      if(distance > playerController.relayEffectDistance) {
        playerController.unregisterRelayPoint(this);
      } else {
        playerController.registerRelayPoint(this);
      }

      // update UI
      if(distance < playerController.relayHintDistance) {
        Debug.Log("Draw hint circle");
        drawCircle(transform.position, distance, hintCircleSegment, Color.white);
        drawCircle(
          transform.position,
          playerController.relayEffectDistance,
          hintCircleSegment,
          Color.green);

        currentHintRenderer.enabled = true;
        targetHintRenderer.enabled = true;
        // size
        currentHintGo.transform.localScale = new Vector2(distance, distance) * 100 * 2 / 64;
        targetHintGo.transform.localScale =
          new Vector2(playerController.relayEffectDistance, playerController.relayEffectDistance) * 100 * 2 / 64;
        // alpha
        float alpha =
          1.0f - (Mathf.Max(distance - playerController.relayEffectDistance, 0.0f)
                    / (playerController.relayHintDistance - playerController.relayEffectDistance));
        currentHintRenderer.material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, alpha));
        targetHintRenderer.material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, alpha));
      } else {
        currentHintRenderer.enabled = false;
        targetHintRenderer.enabled = false;
      }
    }

    private static void drawCircle(Vector3 center, float radius, int segments, Color color) {
      float deltaArc = 2 * Mathf.PI / segments;
      for(int i = 0; i < segments; ++i) {
        float startArc = i * deltaArc;
        float endArc = startArc + deltaArc;
        Debug.DrawLine(
          center + new Vector3(Mathf.Sin(startArc), Mathf.Cos(startArc), 0.0f) * radius,
          center + new Vector3(Mathf.Sin(endArc), Mathf.Cos(endArc), 0.0f) * radius,
          color);
      }
    }
  }
}

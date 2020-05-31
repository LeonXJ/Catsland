using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class RelayPoint: MonoBehaviour {

    public float maxCurrentHintZoom = 2.0f;
    public GameObject currentHintGo;
    public GameObject targetHintGo;
    public int hintCircleSegment = 16;
    public float hintChangeSpeed = 3.0f;
    public Color currentReachTargetColor = Color.blue;

    private GameObject playerGo;
    private PlayerController playerController;
    private SpriteRenderer currentHintRenderer;
    private SpriteRenderer targetHintRenderer;

    private float initCurrentHintScale;
    private float targetAlpha = 0.0f;

    void Awake() {
      playerGo = GameObject.FindGameObjectWithTag(Tags.PLAYER);

      playerController = playerGo.GetComponent<PlayerController>();

      currentHintRenderer = currentHintGo.GetComponent<SpriteRenderer>();
      targetHintRenderer = targetHintGo.GetComponent<SpriteRenderer>();
    }

    void Start() {
      initCurrentHintScale = currentHintRenderer.transform.localScale.x;
    }

    void Update() {
      if(!playerController.supportRelay) {
        return;
      }
      Vector2 delta = playerController.relayDeterminePoint.position - gameObject.transform.position;
      float distance = delta.magnitude;

      // update controller info
      if(distance > playerController.relayHintDistance) {
        playerController.unregisterRelayPoint(this);
      } else {
        playerController.registerRelayPoint(this);
      }

      targetAlpha = 0.0f;
      // update UI
      if(distance < playerController.relayHintDistance && playerController.isNearestRelayPoint(this)) {
        currentHintRenderer.enabled = true;
        targetHintRenderer.enabled = true;

        float progress = 1.0f - Mathf.Max(distance - playerController.relayEffectDistance, 0.0f) / (playerController.relayHintDistance - playerController.relayEffectDistance);
        float scale = Mathf.Lerp(maxCurrentHintZoom * initCurrentHintScale, initCurrentHintScale, progress);
        // size
        currentHintGo.transform.localScale = new Vector2(scale, scale);
        // alpha
        targetAlpha = progress;
      } else {
        HideCircle();
      }

      bool currentReachTarget = distance < playerController.relayEffectDistance;

      // update alpha
      float currentAlpha = Mathf.Lerp(
        currentHintRenderer.material.GetColor(Materials.MATERIAL_ATTRIBUTE_TINT).a, targetAlpha, Time.deltaTime * hintChangeSpeed);
      currentHintRenderer.material.SetColor(
        Materials.MATERIAL_ATTRIBUTE_TINT, currentReachTarget ?  new Color(currentReachTargetColor.r, currentReachTargetColor.g, currentReachTargetColor.b, currentAlpha) : new Color(1.0f, 1.0f, 1.0f, currentAlpha));
      targetHintRenderer.material.SetColor(Materials.MATERIAL_ATTRIBUTE_TINT, new Color(1.0f, 1.0f, 1.0f, currentAlpha));
    }

    public void HideCircle() {
      targetAlpha = 0.0f;
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

    private void OnDestroy() {
      playerController.unregisterRelayPoint(this);
    }
  }
}

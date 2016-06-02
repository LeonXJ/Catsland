using UnityEngine;
using System.Collections;


namespace Catslandx {
  [RequireComponent(typeof(Collider2D))]
  public class RelayPoint : MonoBehaviour {

    public SpriteRenderer lightRender;
    public float enlightDurationInS = 0.5f;
    public float delightDurationInS = 1.0f;

    private float enlightRatio = 0.0f;
    private bool isActive = false;

    private SpriteRenderer spriteRender;

    private static Color ENABLE_COLOR = Color.red;
    private static Color DISABLE_COLOR = Color.white;
    // Use this for initialization
    void Start() {
      spriteRender = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
      if (isActive && enlightRatio < 1.0f) {
        enlightRatio = Mathf.Min(1.0f, enlightRatio + Time.deltaTime / enlightDurationInS);
      } else if (!isActive && enlightRatio > 0.0f) {
        enlightRatio = Mathf.Max(0.0f, enlightRatio - Time.deltaTime / delightDurationInS);
      }
      if (lightRender != null) {
        Color color = lightRender.material.color;
        lightRender.material.color = new Color(color.r, color.g, color.b, enlightRatio);
      }
    }

    void OnTriggerEnter2D(Collider2D other) {
      IRelayPointCatcher catcher = other.GetComponent<IRelayPointCatcher>();
      if(catcher != null && catcher.isSupportRelay()) {
        if(catcher.setRelayPoint(this)) {
          isActive = true;
        }
      }
    }

    void OnTriggerExit2D(Collider2D other) {
      IRelayPointCatcher catcher = other.GetComponent<IRelayPointCatcher>();
      if(catcher != null && catcher.isSupportRelay()) {
        catcher.cancelRelayPoint(this);
        isActive = false;
      }
    }
  }
}

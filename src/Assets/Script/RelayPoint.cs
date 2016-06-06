using UnityEngine;
using System.Collections;


namespace Catslandx {
  [RequireComponent(typeof(Collider2D))]
  public class RelayPoint : MonoBehaviour {

    public SpriteRenderer lightRender;
    public float enlightDurationInS = 0.5f;
    public float delightDurationInS = 1.0f;

    public SpriteRenderer sparkRender;
    public float sparkDurationInS = 0.2f;
    public float insparkDurationInS = 0.8f;

    public GameObject jumpEventObject;
    private IEvent jumpEvent;

    private float enlightRatio = 0.0f;
    private float sparkRatio = 0.0f;
    private bool hasUnprocessSpark = false;
    private bool isActive = false;

    private SpriteRenderer spriteRender;

    // Use this for initialization
    void Start() {
      spriteRender = GetComponent<SpriteRenderer>();
      if (jumpEventObject != null) {
        jumpEvent = jumpEventObject.GetComponent<IEvent>();
      }
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

      if (hasUnprocessSpark && sparkRatio < 1.0f) {
        sparkRatio = Mathf.Min(1.0f, sparkRatio + Time.deltaTime / sparkDurationInS);
        if (1.0f - sparkRatio < Mathf.Epsilon) {
          hasUnprocessSpark = false;
        }
      } else if (sparkRatio > Mathf.Epsilon) {
        sparkRatio = Mathf.Max(0.0f, sparkRatio - Time.deltaTime / insparkDurationInS);
      }
      if (sparkRender != null) {
        Color color = sparkRender.material.color;
        sparkRender.material.color = new Color(color.r, color.g, color.b, sparkRatio);
      }
    }

    public void jumpOnRelay(GameObject gameObject) {
      performOnRelay(gameObject);
      if (jumpEvent != null) {
        jumpEvent.trigger(gameObject);
      }
    }

    public void dashOnRelay(GameObject gameObject) {
      performOnRelay(gameObject);
    }

    private void performOnRelay(GameObject gameObject) {
      hasUnprocessSpark = true;
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

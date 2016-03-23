using UnityEngine;
using System.Collections;


namespace Catslandx {
  [RequireComponent(typeof(Collider2D))]
  public class RelayPoint : MonoBehaviour {

    private SpriteRenderer spriteRender;

    private static Color ENABLE_COLOR = Color.red;
    private static Color DISABLE_COLOR = Color.white;
    // Use this for initialization
    void Start() {
      spriteRender = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter2D(Collider2D other) {
      IRelayPointCatcher catcher = other.GetComponent<IRelayPointCatcher>();
      if(catcher != null && catcher.isSupportRelay()) {
        if(catcher.setRelayPoint(this)) {
          if(spriteRender != null) {
            spriteRender.color = ENABLE_COLOR;
          }
        }
      }
    }

    void OnTriggerExit2D(Collider2D other) {
      IRelayPointCatcher catcher = other.GetComponent<IRelayPointCatcher>();
      if(catcher != null && catcher.isSupportRelay()) {
        catcher.cancelRelayPoint(this);
        if (spriteRender != null) {
          spriteRender.color = DISABLE_COLOR;
        }
      }
    }
  }
}

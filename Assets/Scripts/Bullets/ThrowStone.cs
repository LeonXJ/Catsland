using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;
using Catsland.Scripts.Misc;

namespace Catsland.Scripts.Bullets {

  [RequireComponent(typeof(Rigidbody2D))]
  public class ThrowStone: MonoBehaviour {

    public int damageValue = 1;
    public float repelIntensive = 80.0f;
    public float delaySecond = 1.0f;

    private Rigidbody2D rb2d;
    private bool hasHit = false;

    private void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnCollisionEnter2D(Collision2D collision) {
      onHit(collision);
    }

    private void OnCollisionStay2D(Collision2D collision) {
      onHit(collision);
    }

    private void onHit(Collision2D collision) {
      if (hasHit) {
        return;
      }

      // Collide ground.
      if(collision.gameObject.layer == Layers.LayerGround) {
        hasHit = true;
        FakeOutAndDestroy();
      }

      // Collide player
      if(collision.gameObject.tag == Tags.PLAYER && !hasHit) {
        hasHit = true;
        collision.gameObject.SendMessage(
          MessageNames.DAMAGE_FUNCTION,
          new DamageInfo(
            damageValue, collision.collider.bounds.center, rb2d.velocity, repelIntensive),
          SendMessageOptions.DontRequireReceiver);
        FakeOutAndDestroy();
      }
    }

    void FakeOutAndDestroy() {
      FadeoutAndDestory fadeoutAndDestory = gameObject.AddComponent<FadeoutAndDestory>();
      fadeoutAndDestory.fadeOutInSeconds = delaySecond;
    }


  }
}


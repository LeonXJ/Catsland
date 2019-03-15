using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Bullets {

  [RequireComponent(typeof(Rigidbody2D))]
  public class ThrowStone: MonoBehaviour {

    public int damageValue = 1;
    public float repelIntensive = 80.0f;
    public float delaySecond = 1.0f;

    private Rigidbody2D rb2d;

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
      // Collide ground.
      if(collision.gameObject.layer == Layers.LayerGround) {
        StartCoroutine(waitAndDestroy());
      }

      // Collide player
      if(collision.gameObject.tag == Tags.PLAYER) {
        collision.gameObject.SendMessage(
          MessageNames.DAMAGE_FUNCTION,
          new DamageInfo(
            damageValue, collision.collider.bounds.center, rb2d.velocity, repelIntensive),
          SendMessageOptions.DontRequireReceiver);
      }
    }

    IEnumerator waitAndDestroy() {
      yield return new WaitForSeconds(delaySecond);
      Destroy(gameObject);
    }

  }
}


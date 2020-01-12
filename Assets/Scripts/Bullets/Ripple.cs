using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Bullets {

  [ExecuteInEditMode]
  [RequireComponent(typeof(ParticleSystem), typeof(Rigidbody2D))]
  public class Ripple : MonoBehaviour {

    public int damage = 1;
    public float repelIntense = 10f;
    public float destroyAfterHitGroundInSeconds = 1f;

    private bool hasApplied = false;

    private Rigidbody2D rb2d;
    private new ParticleSystem particleSystem;


    // Start is called before the first frame update
    void Start() {
      rb2d = GetComponent<Rigidbody2D>();
      particleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update() {
      if (Input.GetKey(KeyCode.Z)) {
        rb2d.velocity -= new Vector2(1f, 0);
      }
      if (Input.GetKey(KeyCode.X)) {
        rb2d.velocity += new Vector2(1f, 0);
      }

    }

    void OnTriggerEnter2D(Collider2D other) {
      onRippleHit(other);
    }

    void OnTriggerStay2D(Collider2D other) {
      onRippleHit(other);
    }

    private void onRippleHit(Collider2D other) {
      if (other.gameObject.layer != Layers.LayerGround) {
        return;
      }
      enterExitStage();
    }

    private void enterExitStage() {
      rb2d.velocity = Vector2.zero;
      Destroy(gameObject, particleSystem.main.startLifetime.Evaluate(.5f));
    }

    void OnParticleCollision(GameObject other) {
      Debug.Log("Particle Collide");
      if (hasApplied || !other.CompareTag(Tags.PLAYER)) {
        return;
      }

      // Is player and bullet hasn't applied before.
      GameObject player = SceneConfig.getSceneConfig().GetPlayer();
      player.SendMessage(
        MessageNames.DAMAGE_FUNCTION,
        new DamageInfo(damage, player.transform.position, Vector2.up, repelIntense),
        SendMessageOptions.DontRequireReceiver);
      hasApplied = true;
    }
  }
}

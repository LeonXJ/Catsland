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
    public LayerMask rippleLayerMask;

    private bool hasApplied = false;
    private bool isExiting = false;

    private Rigidbody2D rb2d;
    private new ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;
    private AudioSource audioSource;



    // Start is called before the first frame update
    void Start() {
      rb2d = GetComponent<Rigidbody2D>();
      particleSystem = GetComponent<ParticleSystem>();
      audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
      /*
      if (Input.GetKey(KeyCode.Z)) {
        rb2d.velocity -= new Vector2(1f, 0);
      }
      if (Input.GetKey(KeyCode.X)) {
        rb2d.velocity += new Vector2(1f, 0);
      }
      */
      detectCollision();

      if (isExiting) {
        audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.deltaTime);
      }
    }

    void OnTriggerEnter2D(Collider2D collider) {
      onRippleHit(collider);
    }
    void OnTriggerStay2D(Collider2D collider) {
      onRippleHit(collider);
    }

    private void onRippleHit(Collider2D other) {
      if (other.gameObject.layer != Layers.LayerGround) {
        return;
      }
      enterExitStage();
    }

    private void enterExitStage() {
      rb2d.velocity = Vector2.zero;
      isExiting = true;
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

    private void detectCollision() {
      if (particles == null || particles.Length < particleSystem.main.maxParticles) {
        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
      }
      int aliveParticleNum = particleSystem.GetParticles(particles);
      for (int i = 0; i < aliveParticleNum; i++) {
        ParticleSystem.Particle particle = particles[i];
        Vector3 position = particle.position;
        Vector3 size = particle.GetCurrentSize3D(particleSystem);
        Rect rect = new Rect(position + new Vector3(0f, size.y * .25f, 0f), size * .5f);

        Common.Utils.drawRectAsDebug(rect, Color.blue);
        foreach (Collider2D collider in Physics2D.OverlapBoxAll(rect.position, rect.size, 0, rippleLayerMask)) {
          if (collider.gameObject.layer == Layers.LayerCharacter
            && collider.gameObject.CompareTag(Tags.PLAYER)
            && !hasApplied) {
            collider.gameObject.SendMessage(
              MessageNames.DAMAGE_FUNCTION,
              new DamageInfo(damage, collider.gameObject.transform.position, Vector2.up, repelIntense),
              SendMessageOptions.DontRequireReceiver);
            hasApplied = true;
          }
        }
      }
    }
  }
}

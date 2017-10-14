using UnityEngine;
using System.Collections;
using System;

namespace Catslandx {
  public class DelayRespawn :MonoBehaviour, IRespawn {

    public float delayInS = 1.0f;
    public GameObject respawnPosition = null;
    private CharacterVulnerable characterVulnerable;
    private new SpriteRenderer renderer;
    private ICharacterController2D controller;

    private float currentTimeToRespawn = -1.0f;

    public void doRespawn() {
      currentTimeToRespawn = delayInS;
      renderer.enabled = false;
    }

    public void setRespawn(GameObject respawnPosition) {
      this.respawnPosition = respawnPosition;
    }

    private void respawn() {
      if (respawnPosition != null) {
        GetComponent<Rigidbody2D>().MovePosition(MathHelper.getXY(respawnPosition.transform.position));
        if (characterVulnerable != null) {
          characterVulnerable.respawn();
        }
        if (controller != null) {
          controller.reset();
        }
        renderer.enabled = true;
      }
    }

    // Use this for initialization
    void Start() {
      characterVulnerable = GetComponent<CharacterVulnerable>();
      renderer = GetComponentInChildren<SpriteRenderer>();
      controller = GetComponent<ICharacterController2D>();
    }

    // Update is called once per frame
    void Update() {
      if (currentTimeToRespawn > 0.0f) {
        float deltaTime = Time.deltaTime;
        if (currentTimeToRespawn < deltaTime) {
          respawn();
        }
        currentTimeToRespawn -= deltaTime;
      }
    }
  }
}

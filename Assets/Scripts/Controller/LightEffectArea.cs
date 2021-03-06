﻿using UnityEngine;
using Catsland.Scripts.Camera;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class LightEffectArea : MonoBehaviour {

    public GlobalLightConfig globalColorConfig;

    public float playerDetectionHoldTime = .1f;

    // To be refactor to multiple controller and config pairs
    public SpriteColorStackEffectController spriteColorStackEffectController;
    public SpriteColorConfig spriteColorConfig;

    private GlobalLightController globalLightController;
    private float lastPlayerDetectTime = 0f;
    private bool isPlayerDetected = false;

    public void Awake() {
      globalLightController = GameObject.FindGameObjectWithTag(Tags.SCENE_CONFIG)
        .GetComponent<SceneConfig>()
        .globalLightController;
    }


    private void OnTriggerEnter2D(Collider2D collision) {
      if (collision.CompareTag(Tags.PLAYER)) {
        onPlayerEnter();
      }
    }

    private void OnTriggerExit2D(Collider2D collision) {
      if (collision.CompareTag(Tags.PLAYER)) {
        onPlayerExit();
      }
    }

    // Problem of the following solution:
    // OnTriggerStay2D won't be triggerred if player doesn't move.
    /*
    private void Update() {
      if (isPlayerDetected) {
        if (lastPlayerDetectTime + playerDetectionHoldTime < Time.time) {
          isPlayerDetected = false;
          onPlayerExit();
        }
      }
    }

    public void OnTriggerStay2D(Collider2D collision) {
      if (collision.CompareTag(Tags.PLAYER)) {
        lastPlayerDetectTime = Time.time;
        if (!isPlayerDetected) {
          onPlayerEnter();
        }
        isPlayerDetected = true;
      }
    }
    */

    private void onPlayerEnter() {
      if (globalColorConfig != null) {
        globalLightController.RegisterConfig(globalColorConfig);
      }
      if (spriteColorStackEffectController != null & spriteColorConfig != null) {
        spriteColorStackEffectController.RegisterConfig(spriteColorConfig);
      }
    }

    private void onPlayerExit() {
      if (globalColorConfig != null) {
        globalLightController.UnregisterConfig(globalColorConfig);
      }
      if (spriteColorStackEffectController != null && spriteColorConfig != null) {
        spriteColorStackEffectController.UnregisterConfig(spriteColorConfig);
      }
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller.Mine {
  public class PromixityMine : MonoBehaviour {

    /// <summary>
    /// detect [1x detect ani] -> trigger [min flash speed] -> (delay) -> [max flash speed] explode
    /// </summary>
    [Header("Detect")]
    public float detectRange = 3f;
    public float triggerRange = 2f;
    public float triggerMinFlashSpeed = 1.5f;
    public float triggerMaxFlashSpeed = 5f;
    public float triggerDelay = 1f;

    private bool isTriggered = false;
    private float triggerTime;

    public bool isActive = false;
    public bool isDetect = false;
    public float flashSpeed = 1f;

    [Header("Explode")]
    public GameObject explosionPrefab;

    private const string PAR_ACTIVE = "Active";
    private const string PAR_DETECT = "Detect";

    // References
    private Animator animator;

    // Start is called before the first frame update
    void Start() {
      animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {

      // If active and triggered. Do not care detect.
      flashSpeed = 1f;
      if (shouldContinueTriggerProcess()) {
        float triggerProgress = Mathf.Clamp01((Time.time - triggerTime) / triggerDelay);
        flashSpeed = Mathf.Lerp(triggerMinFlashSpeed, triggerMaxFlashSpeed, triggerProgress);

        if (Time.time - triggerTime > triggerDelay) {
          Explode();
        }
      }

      isDetect = false;
      if (shouldDetect()) {
        GameObject player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
        if (player != null) {
          Vector2 delta = player.transform.position - transform.position;
          float distanceSqrt = delta.sqrMagnitude;
          isDetect = distanceSqrt < detectRange * detectRange;

          // Only update trigger info on first trigger.
          if (!isTriggered && distanceSqrt < triggerRange * triggerRange) {
            isTriggered = true;
            triggerTime = Time.time;
          }
        }
      }

      // Animator
      animator.SetBool(PAR_ACTIVE, isActive);
      animator.SetBool(PAR_DETECT, isDetect);
      animator.speed = flashSpeed;
    }

    private void Explode() {
      GameObject explosionGo = Instantiate(explosionPrefab);
      explosionGo.transform.position = new Vector3(transform.position.x, transform.position.y, explosionGo.transform.position.z);
      Explosion explosion = explosionGo.GetComponent<Explosion>();
      explosion.StartTimer();

      Destroy(gameObject);
    }

    private void OnDrawGizmos() {
      Common.Utils.DrawCircleAsGizmos(detectRange, Color.yellow, transform.position);
      Common.Utils.DrawCircleAsGizmos(triggerRange, Color.red, transform.position);
    }

    private bool shouldContinueTriggerProcess() {
      return isActive && isTriggered;
    }

    private bool shouldDetect() {
      return isActive;
    }
  }
}

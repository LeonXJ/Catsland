﻿using System.Collections;
using UnityEngine;

using Catsland.Scripts.Common;

namespace Catsland.Scripts.Bullets {

  [RequireComponent(typeof(Rigidbody2D))]
  public class ArrowCarrier :MonoBehaviour {


    public int damage = 1;
    public float repelIntensive = 1.0f;
    public string tagForAttachable = "";
    public bool isAttached = false;
    private string tagForOwner;
    private bool isDestroyed = false;
    private Vector2 velocity;

    // References
    private Rigidbody2D rb2d;

    public void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
    }

    public void Update() {
      if(!isAttached) {
        rb2d.velocity = new Vector2(velocity.x, rb2d.velocity.y);
      }
    }

    public IEnumerator fire(Vector2 direction, float lifetime, string tagForOwner) {
      this.tagForOwner = tagForOwner;

      // velocity and orientation
      rb2d.velocity = direction;
      velocity = direction;
      transform.localScale = new Vector2(
        direction.x > 0.0f
            ? Mathf.Abs(transform.localScale.x)
            : -Mathf.Abs(transform.localScale.x),
        1.0f);

      // self destory
      yield return new WaitForSeconds(lifetime);
      if(!isAttached) {
        safeDestroy();
      }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
      // bullet will not distroy bullet
      if(collision.gameObject.layer == gameObject.layer) {
        return;
      }
      if(!isAttached) {
        if(collision.gameObject.CompareTag(tagForAttachable)) {
          isAttached = true;
          rb2d.isKinematic = true;
          rb2d.velocity = Vector2.zero;
          // attach to the object
          gameObject.transform.parent = collision.gameObject.transform;
          // enable one side platform
          GetComponent<Collider2D>().usedByEffector = true;
        } else {
          if(tagForOwner == null || !collision.gameObject.CompareTag(tagForOwner)) {
            collision.gameObject.SendMessage(
              MessageNames.DAMAGE_FUNCTION,
              new DamageInfo(damage, rb2d.velocity, repelIntensive),
              SendMessageOptions.DontRequireReceiver);
            safeDestroy();
          }
        }
      }
    }

    private void safeDestroy() {
      if(!isDestroyed) {
        isDestroyed = true;
        Destroy(gameObject);
      }
    }
  }
}

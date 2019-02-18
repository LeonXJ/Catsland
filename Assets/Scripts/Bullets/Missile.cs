using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Controller;


namespace Catsland.Scripts.Bullets {
  [RequireComponent(typeof(Rigidbody2D))]
  public class Missile: MonoBehaviour {

    public enum Status {
      PREPARE = 0,
      RUNNING,
    }

    public GameObject target;
    public float maxTurningAngleInDegree;
    public Status status = Status.RUNNING;

    public GameObject ignitePrefab;

    public float igniteDistance = 0.3f;

    private Rigidbody2D rb2d;

    private void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
    }

    public void Fire(GameObject target, Vector2 velocity, float lifetime) {
      this.target = target;
      rb2d.velocity = velocity;
      status = Status.RUNNING;
      StartCoroutine(expireAndDestroy(lifetime));
    }

    // Update is called once per frame
    void Update() {
      if(status == Status.RUNNING) {

        // Ignite
        Vector2 delta = target.transform.position - transform.position;
        if(Vector2.SqrMagnitude(delta) < igniteDistance * igniteDistance) {
          ignite();
          return;
        }

        // Moving
        if(target != null) {
          Vector2 v = rb2d.velocity;
          float allowedMaxTurningDegree = maxTurningAngleInDegree * Time.deltaTime;
          float deltaDegree = Vector2.SignedAngle(v, delta);
          float turningDegree = Mathf.Clamp(deltaDegree, -allowedMaxTurningDegree, allowedMaxTurningDegree);
          rb2d.velocity = Quaternion.Euler(0.0f, 0.0f, turningDegree) * v;
          transform.rotation = Quaternion.FromToRotation(Vector2.right, rb2d.velocity);
        }
      }
    }

    void ignite() {
      GameObject explosionGo = Instantiate(ignitePrefab);
      explosionGo.transform.position = new Vector3(transform.position.x, transform.position.y, explosionGo.transform.position.z);
      Explosion explosion = explosionGo.GetComponent<Explosion>();
      explosion.StartTimer();

      // Pass on the relay point
      RelayPoint relay = GetComponentInChildren<RelayPoint>();
      if(relay != null) {
        relay.gameObject.transform.parent = explosionGo.transform;
      }

      safeDestroy();
    }

    private IEnumerator expireAndDestroy(float lifetime) {
      yield return new WaitForSeconds(lifetime);
      ignite();
    }

    private void safeDestroy() {
      if(gameObject != null) {
        Destroy(gameObject);
      }
    }
  }
}

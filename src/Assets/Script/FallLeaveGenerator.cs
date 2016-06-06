using UnityEngine;
using System.Collections;
using System;

namespace Catslandx {
  public class FallLeaveGenerator :MonoBehaviour, IEvent {

    public GameObject leafPrefab;
    public int leaveNumer = 3;
    public float minSpeed = 0.1f;
    public float maxSpeed = 0.4f;
    public Vector2 generaterSize = new Vector2(1.0f, 1.0f);

    public float minAnimationSpeed = 0.5f;

    // Use this for initialization
    void Start() {

    }

    private void generateLeaf() {
      if(leafPrefab != null) {
        Vector2 position = new Vector2(transform.position.x, transform.position.y)
          + MathHelper.multiple(new Vector2(UnityEngine.Random.value, UnityEngine.Random.value), generaterSize)
          - generaterSize * 0.5f;
        GameObject leaf = Instantiate(leafPrefab);
        Falling falling = leaf.GetComponent<Falling>();
        if (falling != null) {
          falling.speed = minSpeed + (maxSpeed - minSpeed) * UnityEngine.Random.value;
        }
        Animator animator = leaf.GetComponent<Animator>();
        if(animator != null) {
          animator.speed = minAnimationSpeed + (1.0f - minAnimationSpeed) * UnityEngine.Random.value;
        }
        leaf.transform.position = position;
      }
    }

    private void generateLeave() {
      for (int i = 0; i<leaveNumer; ++i) {
        generateLeaf();
      }
    }

    void IEvent.trigger(GameObject gameObject) {
      generateLeave();
    }

    // Update is called once per frame
    void Update() {

    }
  }
}

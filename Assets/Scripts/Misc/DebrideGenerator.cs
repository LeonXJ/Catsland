using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Misc {
  public class DebrideGenerator : MonoBehaviour {
    public GameObject debridePrefab;
    public float initialSpeed = .1f;
    public float randomAngleRange = 360f;

    public void GenerateDebrides() => GenerateDebrides(Vector2.right);

    public void GenerateDebrides(Vector2 direction) {
      GameObject debride = Instantiate(debridePrefab);
      debride.transform.position = transform.position;
      // Mostly for orientation.
      debride.transform.localScale = transform.lossyScale;

      if (initialSpeed > Mathf.Epsilon) {
        // Only speed on leaf node
        RecursiveApplyForce(debride, direction.normalized);
      }
    }

    private void RecursiveApplyForce(GameObject go, Vector2 direction) {
      Rigidbody2D rb2d = go.GetComponent<Rigidbody2D>();
      if (rb2d != null) {
        float angle = Random.Range(0f, randomAngleRange) - randomAngleRange * .5f;
        Vector2 actualDirection = Quaternion.Euler(0f, 0f, angle) * direction;
        rb2d.velocity = actualDirection * initialSpeed;
      }
      for (int i = 0; i < go.transform.childCount; i++) {
        RecursiveApplyForce(go.transform.GetChild(i).gameObject, direction);
      }
    }
  }
}

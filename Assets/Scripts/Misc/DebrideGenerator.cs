using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Misc {
  public class DebrideGenerator : MonoBehaviour {
    public GameObject debridePrefab;
    public float initialSpeed = .1f;

    public void GenerateDebrides() {
      GameObject debride = Instantiate(debridePrefab);
      debride.transform.position = transform.position;

      if (initialSpeed > Mathf.Epsilon) {
        // Only speed on leaf node
        RecursiveApplyForce(debride);
      }
    }

    private void RecursiveApplyForce(GameObject go) {
      if (go.transform.childCount == 0) {
        // child node
        Rigidbody2D rb2d = go.GetComponent<Rigidbody2D>();
        if (rb2d != null) {
          float angle = Random.Range(0f, 360f);
          Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
          rb2d.velocity = direction * initialSpeed;
        }
      } else {
        for (int i = 0; i < go.transform.childCount; i++) {
          RecursiveApplyForce(go.transform.GetChild(i).gameObject);
        }
      }
    }
  }
}

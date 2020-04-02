using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class SlimeSilhouette : MonoBehaviour {

    public GameObject slimePrefab;
    public float slimeZ = 0f;

    private Rigidbody2D rb2d;

    private void Start() {
      rb2d = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update() {
      if (rb2d.velocity.y < 0f) {
        GameObject slime = Instantiate(slimePrefab);
        slime.transform.position = new Vector3(transform.position.x, transform.position.y, slimeZ);
        SceneConfig.getSceneConfig().arenaDirector?.addOpponent(slime);

        Destroy(gameObject);
      }
    }
  }
}

using Catsland.Scripts.Bullets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Fx {
  public class WaterSplashGenerator : MonoBehaviour {

    public GameObject splashPrefab;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter2D(Collider2D collision) {
      GameObject splash = Instantiate(splashPrefab);
      splash.transform.position = collision.transform.position;
      splash.GetComponent<ParticleSystem>()?.Play();
      Destroy(splash, 1f);

      collision.gameObject.GetComponent<ArrowCarrier>()?.HitWater();
    }
  }
}

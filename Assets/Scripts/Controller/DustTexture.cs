using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class DustTexture : MonoBehaviour {

    public bool hasDust = true;
    public Color startColor = Color.red;

    void OnTriggerEnter2D(Collider2D collider) {
      if (collider.tag == Tags.PLAYER) {
        ParticleSystem particleSystem = collider.gameObject.GetComponent<ParticleSystem>();
        if (particleSystem != null) {
          ApplyTexture(particleSystem);
        }
      }
    }

    private void ApplyTexture(ParticleSystem particleSystem) {
      var emissionModule = particleSystem.emission;
      emissionModule.enabled = hasDust;
      if (hasDust) {
        var mainModule = particleSystem.main;
        mainModule.startColor = startColor;
      }
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class DustTexture : MonoBehaviour {

    public interface DustTextureAssignee {
      void AssignDustTexture(DustTexture dustTexture);
    }

    public bool hasDust = true;
    public ParticleSystem.MinMaxGradient dustColor;

    void OnTriggerEnter2D(Collider2D collider) {
      if (collider.tag == Tags.PLAYER) {
        DustTextureAssignee assignee = collider.gameObject.GetComponent<DustTextureAssignee>();
        if (assignee != null) {
          assignee.AssignDustTexture(this);
        }
      }
    }

    public void ApplyTexture(ParticleSystem particleSystem) {
      var emissionModule = particleSystem.emission;
      emissionModule.enabled = hasDust;
      if (hasDust) {
        var colorOverLifetimeModule = particleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = dustColor;
      }
    }
  }
}

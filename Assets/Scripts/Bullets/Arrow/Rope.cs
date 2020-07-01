using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Catsland.Scripts.Bullets.Arrow {
  public class Rope : MonoBehaviour {

    public float springStrength = 100f;
    public float springEffectTime = 1f;
    public float minEffectDistance = 0.5f;

    private bool hasStartShoot = false;
    private float effectTimeRemaining = 0f;
    private float previousDeltaSqr = float.MaxValue;

    private Transform attachedVisualTransform;
    private GameObject attachedPhysicsGo;
    private Transform ownerVisualTransform;
    private GameObject ownerPhysicsGo;
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start() {
      lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update() {
      if (hasStartShoot) {
        // if attachedGo == null || ownerGo == null
        // self-disposal
        if (attachedVisualTransform == null || ownerVisualTransform == null) {
          dispose();
          return;
        }
        lineRenderer.SetPosition(0, Common.Utils.toVector2(attachedVisualTransform.position));
        lineRenderer.SetPosition(1, Common.Utils.toVector2(ownerVisualTransform.position));
      }

      if (effectTimeRemaining > 0f) {
        bool canApplyForce = applyForce();
        effectTimeRemaining -= Time.deltaTime;
        if (effectTimeRemaining < 0f || !canApplyForce) {
          dispose();
        }
      }
    }

    public void onShoot(
      Transform ownerVisualTransform,
      GameObject ownerPhysicsGo, 
      Transform attachedVisualTransform,
      GameObject attachedPhysicsGo) {
      this.ownerVisualTransform = ownerVisualTransform;
      this.ownerPhysicsGo = ownerPhysicsGo;
      this.attachedVisualTransform = attachedVisualTransform;
      this.attachedPhysicsGo = attachedPhysicsGo;
      hasStartShoot = true;
      lineRenderer.enabled = true;
    }


    public void setAttachedVisualTransform(Transform attachedVisualTransform) {
      this.attachedVisualTransform = attachedVisualTransform;
    }

    public void setAttachedPhysicsGo(GameObject attachedPhysicsGo) {
      this.attachedPhysicsGo = attachedPhysicsGo;
    }

    public void startEffect() {
      Debug.Assert(hasStartShoot, "Cannot start effect before shooting");
      effectTimeRemaining = springEffectTime;
    }

    public bool applyForce() {
      if (ownerVisualTransform == null || attachedVisualTransform == null) {
        return false;
      }

      Vector2 delta = attachedVisualTransform.position - ownerVisualTransform.position;
      float deltaSqr = delta.sqrMagnitude;
      if (deltaSqr < minEffectDistance * minEffectDistance
        || deltaSqr > previousDeltaSqr) {
        return false;
      }
      previousDeltaSqr = deltaSqr;
      Vector2 direction = delta.normalized;

      applyForceToPhysicsGo(ownerPhysicsGo, direction * springStrength);
      applyForceToPhysicsGo(attachedPhysicsGo, -direction * springStrength);

      return true;
      // self-disposal
      //dispose();
    }

    private void applyForceToPhysicsGo(GameObject physicsGo, Vector2 force) {
      Rigidbody2D rb2d = physicsGo.GetComponent<Rigidbody2D>();
      if (rb2d!= null) {
        rb2d.AddForce(force);
      }
      physicsGo.SendMessage(Common.MessageNames.APPLY_ROPE_FORCE, force, SendMessageOptions.DontRequireReceiver);
    }

    private void dispose() {
      lineRenderer.enabled = false;
      effectTimeRemaining = -1f;
    }
  }
}

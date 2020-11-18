using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Catsland.Scripts.Bullets {
  public class Utils {

    public const float MAX_KNOCKBACK_SPEED = 16f;
    public const float KNOCKBACK_DRAG = 0.3f;

    public static void ApplyRepel(DamageInfo damageInfo, Rigidbody2D rb2d, float maxRepelForce = float.MaxValue) {
      rb2d.velocity = Vector2.zero;
      rb2d.AddForce(damageInfo.repelDirection.normalized * Mathf.Min(damageInfo.repelIntense, maxRepelForce));
    }

    public static void ApplyVelocityRepel(Vector2 velocity, Rigidbody2D rb2d) {
      rb2d.velocity = velocity;
    }

    public static IEnumerator ApplyVelocityRepel(
      DamageInfo damageInfo, Rigidbody2D rb2d, float dizzyTimeInS,
      float knockbackCoherence = 1f, float maxKnockbackSpeed = MAX_KNOCKBACK_SPEED,
      float knockbackDrag = KNOCKBACK_DRAG) {

      rb2d.velocity = damageInfo.repelDirection.normalized
        * Mathf.Clamp(damageInfo.repelIntense * knockbackCoherence, 0f, maxKnockbackSpeed);

      rb2d.drag = knockbackDrag;

      yield return new WaitForSeconds(dizzyTimeInS);

      rb2d.velocity = Vector2.zero;
      rb2d.drag = 0f;
    }


    public class DamageHelper {

      public delegate void OnDie(DamageInfo damageInfo);

      private MonoBehaviour controller;
      private VulnerableAttribute vulnerableAttribute;
      private Timing timing;
      private OnDie onDie;

      // Nullable
      private Rigidbody2D rb2d;
      private Animator animator;

      // Call this when the object is damaged.
      public void OnDamaged(DamageInfo damageInfo) {
        damageInfo.onDamageFeedback?.Invoke(new DamageInfo.DamageFeedback(true));

        vulnerableAttribute.currentHealth -= damageInfo.damage;
        if (vulnerableAttribute.currentHealth <= 0) {
          onDie(damageInfo);
          return;
        }

        controller.StartCoroutine(FreezeThen(timing.ArrowShakeTime, damageInfo));
      }

      private IEnumerator FreezeThen(float time, DamageInfo damageInfo) {

        float repelSpeed = vulnerableAttribute.arrowHitRepelSpeed;
        if (rb2d != null) {
          if (damageInfo.isKnockback()) {
            repelSpeed = vulnerableAttribute.knockbackRepelSpeed;
          }
          if (damageInfo.isSmashAttack) {
            repelSpeed = vulnerableAttribute.strongArrowHitRepelSpeed;
          }

          rb2d.velocity = new Vector2(-Mathf.Sign(Common.Utils.getOrientation(controller.gameObject)) * repelSpeed, rb2d.velocity.y);
          rb2d.bodyType = RigidbodyType2D.Kinematic;
        }
        if (animator != null) {
          animator.speed = 0f;
        }

        controller.transform.DOShakePosition(time, .15f, 30, 120);

        yield return new WaitForSeconds(time);

        if (animator != null) {
          animator.speed = 1f;
        }
        if (rb2d != null) {
          rb2d.bodyType = RigidbodyType2D.Dynamic;
          Utils.ApplyVelocityRepel(damageInfo.repelDirection.normalized * repelSpeed, rb2d);
        }
      }

      private void DefaultOnDie(DamageInfo damageInfo) {
        Object.Destroy(controller.gameObject);
      }
      public class DamageHelperBuilder {
        private readonly DamageHelper damageHelper;

        private DamageHelperBuilder(
          MonoBehaviour controller, VulnerableAttribute vulnerableAttribute, Timing timing) {
          damageHelper = new DamageHelper {
            controller = controller,
            vulnerableAttribute = vulnerableAttribute,
            timing = timing
          };
        }

        public static DamageHelperBuilder NewBuilder(
          MonoBehaviour controller, VulnerableAttribute vulnerableAttribute, Timing timing) {
          return new DamageHelperBuilder(controller, vulnerableAttribute, timing);
        }

        public DamageHelperBuilder SetRigidbody2d(Rigidbody2D rb2d) {
          damageHelper.rb2d = rb2d;
          return this;
        }

        public DamageHelperBuilder SetAnimator(Animator animator) {
          damageHelper.animator = animator;
          return this;
        }

        public DamageHelperBuilder SetTiming(Timing timing) {
          damageHelper.timing = timing;
          return this;
        }

        public DamageHelperBuilder SetOnDie(OnDie onDie) {
          damageHelper.onDie = onDie;
          return this;
        }


        public DamageHelper Build() {
          if (damageHelper.rb2d == null) {
            damageHelper.rb2d = damageHelper.controller.GetComponent<Rigidbody2D>();
          }
          if (damageHelper.animator == null) {
            damageHelper.animator = damageHelper.controller.GetComponent<Animator>();
          }
          if (damageHelper.onDie == null) {
            damageHelper.onDie = damageHelper.DefaultOnDie;
          }
          return damageHelper;
        }
      }
    }
  }
}
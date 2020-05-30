using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.Controller.ShieldMan {

  [RequireComponent(typeof(IInput))]
  [RequireComponent(typeof(Animator))]
  public class ShieldManController : MonoBehaviour, IDamageInterceptor, IMeleeDamageInterceptor {

    // Animation state names.
    private const string ANI_STAND = "ShieldMan_Stand";
    private const string ANI_WALK = "ShieldMan_Walk";

    // Animation parameters.
    private const string PAR_H_SPEED = "HSpeed";
    private const string PAR_IS_ATTACK = "IsAttack";
    private const string PAR_IS_UNBALANCED = "IsUnbalanced";

    [Header("Movement")]
    public float walkAcceleration = 1f;
    public float walkSpeed = 1f;

    [Header("Damage")]
    public float frezeTime = .1f;
    public float arrowRepelSpeed = .1f;
    public float dashRepelSpeed = 1f;
    public float smashRepelSpeed = 1f;
    public float unbalanceTime = 1f;

    private float unbalanceRemainingTime;

    private Rigidbody2D rb2d;
    private Animator animator;
    private IInput input;

    // Start is called before the first frame update
    void Start() {
      rb2d = GetComponent<Rigidbody2D>();
      animator = GetComponent<Animator>();
      input = GetComponent<IInput>();
    }

    // Update is called once per frame
    void Update() {
      // Movement
      float inputHVelocity = input.getHorizontal();
      if (canWalk()) {
        if (inputHVelocity * inputHVelocity > Mathf.Epsilon) {
          rb2d.AddForce(new Vector2(walkAcceleration * inputHVelocity, 0.0f));
          rb2d.velocity = new Vector2(Mathf.Clamp(rb2d.velocity.x, -walkSpeed, walkSpeed), rb2d.velocity.y);
        } else {
          // stand still
          rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
        }
      }

      bool isAttack = false;
      if (canAttack() && input.attack()) {
        isAttack = true;
        rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
      }

      if (canChangeOrientation()) {
        ControllerUtils.AdjustOrientation(inputHVelocity, gameObject);
      }

      if (isUnbalanced()) {
        unbalanceRemainingTime -= Time.deltaTime;
      }

      // Update animation parameters.
      animator.SetFloat(PAR_H_SPEED, Mathf.Abs(rb2d.velocity.x));
      animator.SetBool(PAR_IS_ATTACK, isAttack);
      animator.SetBool(PAR_IS_UNBALANCED, isUnbalanced());
    }

    public void damage(DamageInfo damageInfo) {
      damageInfo.onDamageFeedback?.Invoke(new DamageInfo.DamageFeedback(true));

      StartCoroutine(freezeThen(frezeTime, damageInfo));
    }

    private IEnumerator freezeThen(float time, DamageInfo damageInfo) {
      float repelSpeed = arrowRepelSpeed;
      if (damageInfo.isDash) {
        repelSpeed = dashRepelSpeed;
        unbalanceRemainingTime = unbalanceTime;
      } else if (damageInfo.isSmashAttack) {
        repelSpeed = smashRepelSpeed;
      }

      rb2d.velocity = new Vector2(-Mathf.Sign(damageInfo.repelDirection.x) * repelSpeed * 0.5f, rb2d.velocity.y);
      rb2d.bodyType = RigidbodyType2D.Kinematic;
      animator.speed = 0f;

      transform.DOShakePosition(time, .15f, 30, 120);

      yield return new WaitForSeconds(time);

      animator.speed = 1f;
      rb2d.bodyType = RigidbodyType2D.Dynamic;
      Utils.ApplyVelocityRepel(damageInfo.repelDirection.normalized * repelSpeed, rb2d);
    }

    private bool canWalk() =>
      (animator.GetCurrentAnimatorStateInfo(0).IsName(ANI_STAND)
          || animator.GetCurrentAnimatorStateInfo(0).IsName(ANI_WALK))
        && !isUnbalanced();

    private bool canAttack() => canWalk();

    private bool canChangeOrientation() => canWalk();

    private bool isUnbalanced() => unbalanceRemainingTime > 0f;

    public ArrowResult getArrowResult(ArrowCarrier arrowCarrier) {
      return ArrowResult.HIT;
    }

    public MeleeResult getMeleeResult() {
      return MeleeResult.HIT;
    }
  }
}

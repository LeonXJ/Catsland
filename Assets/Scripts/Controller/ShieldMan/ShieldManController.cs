﻿using System.Collections;
using UnityEngine;
using DG.Tweening;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Common;
using Catsland.Scripts.Ui;

namespace Catsland.Scripts.Controller.ShieldMan {

  [RequireComponent(typeof(IInput))]
  [RequireComponent(typeof(Animator))]
  public class ShieldManController
    : MonoBehaviour, IDamageInterceptor, IMeleeDamageInterceptor, IHealthBarQuery {

    // Animation state names.
    private const string ANI_STAND = "ShieldMan_Stand";
    private const string ANI_WALK = "ShieldMan_Walk";
    private const string ANI_ATTACK = "ShieldMan_Attack";

    // Animation parameters.
    private const string PAR_H_SPEED = "HSpeed";
    private const string PAR_IS_ATTACK = "IsAttack";
    private const string PAR_IS_UNBALANCED = "IsUnbalanced";
    private const string PAR_IS_DEAD = "IsDead";

    private readonly DamageInfo SHIELD_ATTACHED_ARROW_CLEAN = new DamageInfo(1, Vector2.zero, Vector2.zero, 0f);

    [Header("General")]
    public string displayName = "Shield Man";

    [Header("Movement")]
    public float walkAcceleration = 1f;
    public float walkSpeed = 1f;

    [Header("Damage")]
    public float frezeTime = .1f;
    public float arrowRepelSpeed = .1f;
    public float dashRepelSpeed = 1f;
    public float smashRepelSpeed = 1f;
    public float unbalanceTime = 1f;
    public Vector2 dieRepal = new Vector2(-10f, 10f);

    [Header("Hp")]
    public int maxHp = 3;
    public GameObject dieEffectPrefab;
    public Transform dieEffectPosition;
    private int currentHp;

    private float unbalanceRemainingTime;

    private Rigidbody2D rb2d;
    private Animator animator;
    private IInput input;
    private Shield.Shield shield;

    // Start is called before the first frame update
    void Start() {
      rb2d = GetComponent<Rigidbody2D>();
      animator = GetComponent<Animator>();
      input = GetComponent<IInput>();
      shield = GetComponentInChildren<Shield.Shield>();

      currentHp = maxHp;
    }

    // Update is called once per frame
    void Update() {
      // Movement
      float inputHVelocity = input.getHorizontal();
      if (canWalk()) {
        ControllerUtils.ApplyHorizontalVelocity(rb2d, inputHVelocity, walkAcceleration, walkSpeed);


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
      animator.SetBool(PAR_IS_DEAD, isDead());
    }

    public void damage(DamageInfo damageInfo) {
      damageInfo.onDamageFeedback?.Invoke(new DamageInfo.DamageFeedback(true));

      CircleCollider2D headCollider = GetComponent<CircleCollider2D>();
      bool isCriticalHit = (headCollider != null && damageInfo.hitCollider == headCollider);
      if (isCriticalHit) {
        currentHp = 0;
      } else {
        currentHp -= damageInfo.damage;
      }
      if (isDead()) {
        // Die animation will do the rest.
        return;
      }

      // Repel speed
      float repelSpeed = arrowRepelSpeed;
      if (damageInfo.isDash) {
        repelSpeed = dashRepelSpeed;
        unbalanceRemainingTime = unbalanceTime;
      } else if (damageInfo.isKick) {
        repelSpeed = smashRepelSpeed;
        unbalanceRemainingTime = unbalanceTime;
      }

      StartCoroutine(ControllerUtils.freezeThen(
        transform,
        rb2d,
        animator,
        frezeTime,
        new Vector2(-Mathf.Sign(damageInfo.repelDirection.x) * repelSpeed * .5f, rb2d.velocity.y),
        damageInfo.repelDirection.normalized * repelSpeed));
    }

    public void generateDieEffect() {
      if (dieEffectPrefab != null) {
        GameObject dieEffect = Instantiate(dieEffectPrefab);
        dieEffect.transform.position = Vector3Builder.From(dieEffectPosition.position).SetZ(AxisZ.SPLASH).Build();
        dieEffect.GetComponent<ParticleSystem>()?.Play(true);
      }
    }

    public void DestroyObject() {
      Destroy(gameObject);
    }

    public void generateDieRepal() {
      rb2d.AddForce(new Vector2(getOrientation() * dieRepal.x, dieRepal.y));
    }

    public float getOrientation() {
      return transform.lossyScale.x > 0.0f ? 1.0f : -1.0f;
    }

    private bool canWalk() =>
      (animator.GetCurrentAnimatorStateInfo(0).IsName(ANI_STAND)
          || animator.GetCurrentAnimatorStateInfo(0).IsName(ANI_WALK))
        && !isUnbalanced()
        && !isDead();

    private bool canAttack() => canWalk();

    private bool canChangeOrientation() => canWalk();

    private bool isUnbalanced() => unbalanceRemainingTime > 0f;

    private bool isDead() => currentHp <= 0;

    private bool isAttacking() => animator.GetCurrentAnimatorStateInfo(0).IsName(ANI_ATTACK);

    public ArrowResult getArrowResult(ArrowCarrier arrowCarrier) {
      return isDead() ? ArrowResult.SKIP : ArrowResult.HIT;
    }

    public MeleeResult getMeleeResult() {
      return (isDead() || isAttacking()) ? MeleeResult.VOID : MeleeResult.HIT;
    }

    public void onShieldDamage(DamageInfo damageInfo) {
      if (damageInfo.isDash) {
        damage(damageInfo);
      }
    }

    public void cleanShield() {
      if (shield == null) {
        return;
      }
      foreach (AttachedArrow attachedArrow in shield.GetComponentsInChildren<AttachedArrow>()) {
        attachedArrow.damage(SHIELD_ATTACHED_ARROW_CLEAN);
      }
    }

    public HealthCondition GetHealthCondition() {
      return new HealthCondition(maxHp, currentHp, displayName);
    }
  }
}

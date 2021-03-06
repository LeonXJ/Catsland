﻿using UnityEngine;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Misc;

namespace Catsland.Scripts.Controller {
  [RequireComponent(typeof(IInput)), RequireComponent(typeof(Rigidbody2D))]
  public class ShellCareerController: MonoBehaviour, Misc.IDamageInterceptor, Bullets.IDamageInterceptor {

    private static readonly float BROKEN_SHELL_EFFECT_CONVERT_SPEED = 0.1f;

    public float walkingSpeed = 1.0f;
    public int maxHealth = 5;
    public int curHealth;
    public float dormantDurationInS = 5.0f;
    public float enterDormantRepelForce = 10.0f;
    public int maxShellHealth = 2;
    public SpriteRenderer shellBrokenEffectRenderer;
    public float shellReceoverPerS = 1.0f;
    public bool disableContactDamageInDormant = true;

    public GameObject stingGenerations;
    public GameObject stingPrefab;
    public float stingSpeed = 3.0f;
    public float stingGenerationInterval = 1.0f;
    private float stingGenerationTimer = 0.0f;


    private IInput input;
    private Rigidbody2D rb2d;
    private Animator animator;

    private float currentDormantTime = 0.0f;
    private Vector2 lastDamageRepelDirection;
    public int curShellHeath;
    private float shellRecoverSlot = 0.0f;
    private bool isDormant = false;

    // Animation
    private AnimatorStateInfo currentState;

    private static readonly string STATUS_WALK = "Walk";
    private static readonly string IS_DORMANT = "IsDormant";
    private static readonly string ATTACK = "Attack";

    private void Awake() {
      input = GetComponent<IInput>();
      rb2d = GetComponent<Rigidbody2D>();
      animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start() {
      enterIdeal();
    }

    // Update is called once per frame
    void Update() {
      currentState = animator.GetCurrentAnimatorStateInfo(0);

      if (canAttack() && input.attack()) {
        animator.SetTrigger(ATTACK);
      }

      float desiredSpeed = input.getHorizontal();
      if(canWalk()) {
        rb2d.velocity = new Vector2(walkingSpeed * desiredSpeed, rb2d.velocity.y);
      }
      if(canAdjustOrientation()) {
        ControllerUtils.AdjustOrientation(desiredSpeed, gameObject);
      }

      if(isDormant) {
        currentDormantTime += Time.deltaTime;
        if(currentDormantTime > dormantDurationInS) {
          enterIdeal();
        }
      }

      // Auto receover shell
      if(curShellHeath < maxShellHealth) {
        shellRecoverSlot += Time.deltaTime;
        if(shellRecoverSlot > shellReceoverPerS) {
          shellRecoverSlot -= shellReceoverPerS;
          curShellHeath++;
        }
      }

      // Broken shell effect
      if(shellBrokenEffectRenderer != null) {
        float currentAlpha = shellBrokenEffectRenderer.material.GetColor("_Color").a;
        float targetAlpha = 1.0f - ((float)curShellHeath) / maxShellHealth;
        shellBrokenEffectRenderer.material.SetColor(
          "_Color", new Color(1.0f, 1.0f, 1.0f, Mathf.Lerp(currentAlpha, targetAlpha, BROKEN_SHELL_EFFECT_CONVERT_SPEED)));
      }

      if(canGenerateSting()) {
        if(stingGenerationTimer > stingGenerationInterval) {
          generateStings();
        } else {
          stingGenerationTimer += Time.deltaTime;
        }
      } else {
        stingGenerationTimer = 0.0f;
      }

      // Animation
      animator.SetBool(IS_DORMANT, isDormant);
    }

    public bool canWalk() {
      return currentState.IsName(STATUS_WALK);
    }

    public bool canAttack() {
      return currentState.IsName(STATUS_WALK);
    }

    public bool canAdjustOrientation() {
      return currentState.IsName(STATUS_WALK);
    }

    public bool canGenerateSting() {
      return stingGenerations != null && currentState.IsName(STATUS_WALK);
    }

    public void DoAttack() {
      animator.ResetTrigger(ATTACK);
    }

    private void generateStings() {
      for(int i = 0; i < stingGenerations.transform.childCount; i++) {
        Transform t = stingGenerations.transform.GetChild(i);
        GameObject stingGo = Instantiate(stingPrefab);
        stingGo.transform.position = t.position;
        stingGo.transform.rotation = t.rotation;

        stingGo.GetComponent<Spell>().fire(gameObject);

        Vector2 v = t.rotation * Vector2.up * stingSpeed;
        stingGo.GetComponent<Rigidbody2D>().velocity = v;
      }
      stingGenerationTimer = 0.0f;
    }

    private void enterDormant() {
      currentDormantTime = 0.0f;
      isDormant = true;
      // Apply a repel force
      rb2d.AddForce(lastDamageRepelDirection * enterDormantRepelForce);
      // Disable contact damage
      if(disableContactDamageInDormant) {
        foreach(ContactDamage cd in GetComponentsInChildren<ContactDamage>()) {
          cd.enabled = false;
        }
      }
    }

    private void enterIdeal() {
      curHealth = maxHealth;
      isDormant = false;
      curShellHeath = maxShellHealth;
      // Enable contact damage
      foreach(ContactDamage cd in GetComponentsInChildren<ContactDamage>()) {
        cd.enabled = true;
      }
    }

    public void damage(DamageInfo damageInfo) {
      // Check shell
      if(curShellHeath > 0) {
        curShellHeath -= damageInfo.damage;
        return;
      }
      // else take damage.
      lastDamageRepelDirection = damageInfo.repelDirection;
      curHealth -= damageInfo.damage;
      if(curHealth <= 0) {
        enterDormant();
      }
    }

    bool Misc.IDamageInterceptor.shouldFlashOnDamage(DamageInfo damageInfo) {
      return curShellHeath <= 0;
    }

    bool Misc.IDamageInterceptor.shouldSplashOnDamage(DamageInfo damageInfo) {
      return curShellHeath <= 0;
    }

    ArrowResult Bullets.IDamageInterceptor.getArrowResult(ArrowCarrier arrowCarrier) {
      return arrowCarrier.isShellBreaking
        ? (curShellHeath > 0 ? ArrowResult.HIT_AND_BROKEN : ArrowResult.HIT)
        : (curShellHeath > 0 ? ArrowResult.BROKEN : ArrowResult.HIT);
    }
  }
}

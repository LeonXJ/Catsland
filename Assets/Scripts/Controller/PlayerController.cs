using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Cinemachine;

using Catsland.Scripts.Bullets;
using Catsland.Scripts.Misc;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {

  [RequireComponent(typeof(IInput))]
  [RequireComponent(typeof(Animator))]
  public class PlayerController: MonoBehaviour, DustTexture.DustTextureAssignee {

    // Locomoation
    [Header("Run")]
    public float maxRunningSpeed = 1.0f;
    public float maxCrouchSpeed = 0.5f;
    public float acceleration = 1.0f;

    [Header("Jump")]
    public float jumpForce = 5.0f;
    public float doubleJumpForce = 10.0f;
    public float cliffJumpForce = 5.0f;
    public float maxFallingSpeed = 5.0f;
    public float cliffSlidingSpeed = 1.0f;
    public float fallMultiplier = 2.5f;
    public float lowJumMultiplier = 2f;
    public ParticleSystem smashParticleSystem;

    [Header("Dash")]
    public float dashSpeed = 3.0f;
    public float dashTime = 0.6f;
    public float dashCooldown = 1.0f;
    public Vector2 dashCameraShakeScale = new Vector2(0.5f, 0.0f);
    // If true, player orientation will affect the sign of x of dashCameraShakeScale.
    public bool dashCameraShakeApplyOrientationImpact = true;
    public int dashKnowbackParticleNumber = 120;
    public float dashKnowbackTimeslowDuration = 2f;
    public float dashKnowbackTimeslowScale = 0.6f;
    private float dashknowbackTimeslowRemaining = 0f;

    public ParticleSystem dashParticle;
    public ContactDamage dashKnockbackContactDamage;
    public ParticleSystem dashKnockbackParticle;

    [Header("Relay")]
    public Transform relayDeterminePoint;
    public float relayHintDistance = 3.0f;
    public float relayEffectDistance = 1.0f;
    public bool supportRelay = true;
    public bool canCliffJump = true;
    public bool canDetectCliff = true;
    public float timeScaleInRelay = 0.6f;


    public float maxSenseAdd = 0.5f;
    public float senseIncreaseSpeed = 0.2f;
    public float currentSense = 0.0f;

    private bool isCliffSliding;
    private float dashRemainingTime = 0.0f;
    private float gravityScale = 1.0f;
    private int remainingDash = 1;
    private float dashCooldownRemaining = 0.0f;
    private bool isMeditation = false;

    public float smashMinSpeed = 1.0f;
    private bool isLastUpdateOnGround = false;

    // Attack
    [Header("Arrow")]
    public float maxArrowSpeed = 15.0f;
    public float minArrowSpeed = 5.0f;

    public float quickArrowLifetime = 1.0f;
    public float maxArrowLifetime = 3.0f;

    public float maxRepelForce = 800f;
    public float quickRepelForce = 300f;
    public float minDrawingTime = 0.5f;
    public float maxDrawingTime = 2.0f;
    public float strongArrowDrawingRatio = 0.9f;
    public float minTrailTime = 1f;
    public float maxTrailTime = 3f;
    
    private bool isDrawing = false;
    private float shootingCd = 0.5f;
    private bool isShooting = false;
    private float currentDrawingTime = 0.0f;
    private HashSet<RelayPoint> activeRelayPoints = new HashSet<RelayPoint>();
    private RelayPoint nearestRelayPoint;

    // Health
    [Header("Health")]
    public int maxHealth = 3;
    public int currentHealth;
    public float dizzyTime = 1.0f;
    public float timeScaleInDizzy = 0.4f;
    public float immutableTime = 0.5f;
    public int score = 0;

    private bool isDizzy = false;
    private float lastGetDamagedTime = 0.0f;

    // References
    public GameObject groundSensorGO;
    public GameObject headSenserGo;
    public GameObject frontSensorGo;
    public GameObject backSensorGo;
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public TrailIndicator trailIndicator;
    public GameObject cliffJumpEffectPrefab;
    public Transform smashEffectPoint;
    public GameObject smashEffectPrefab;
    public Transform forwardCliffJumpEffectPoint;
    public Transform backwardCliffJumpEffectPoint;
    public GameObject doubleJumpEffectPrefab;
    public Transform doubleJumpEffectPoint;
    public Animator damageEffectAnimator;
    public ParticleSystem damageEffectParticleSystem;

    public Vector3 footPosition {
      get {
        return groundSensorGO.transform.position;
      }
    }


    private ISensor groundSensor;
    private ISensor backSensor;
    private ISensor frontSensor;
    private IInput input;
    private Rigidbody2D rb2d;
    private Animator animator;
    private BoxCollider2D headCollider;
    private SpriteRenderer spriteRenderer;
    private CinemachineImpulseSource cinemachineImpulseSource;

    private GameObject previousParentGameObject;
    private Vector3 previousParentPosition;
    private ParticleSystem dustParticleSystem;

    // Animation
    private const string H_SPEED = "HSpeed";
    private const string V_SPEED = "VSpeed";
    private const string GROUNDED = "Grounded";
    private const string DRAWING = "Drawing";
    private const string DIZZY = "Dizzy";
    private const string CROUCH = "Crouch";
    private const string CLIFF_SLIDING = "CliffSliding";
    private const string DASHING = "Dashing";


    public void Start() {
      input = GetComponent<IInput>();
      rb2d = GetComponent<Rigidbody2D>();
      groundSensor = groundSensorGO.GetComponent<ISensor>();
      frontSensor = frontSensorGo.GetComponent<ISensor>();
      backSensor = backSensorGo.GetComponent<ISensor>();
      animator = GetComponent<Animator>();
      headCollider = GetComponent<BoxCollider2D>();
      spriteRenderer = GetComponent<SpriteRenderer>();
      cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
      dustParticleSystem = GetComponent<ParticleSystem>();
    }

    public void Awake() {
      currentHealth = maxHealth;
      dashKnockbackContactDamage.enabled = false;
      dashKnockbackContactDamage.onHitEvent = onDashKnockbackEvent;
    }

    public void Update() {
      float desiredSpeed = input.getHorizontal();
      float currentVerticleVolocity = rb2d.velocity.y;
      Vector2 appliedForce = Vector2.zero;

      // Relay point update
      updateNearestRelayPoint();

      // Cooldown
      if(dashCooldownRemaining > 0.0f) {
        dashCooldownRemaining -= Time.deltaTime;
      }
      if(!input.meditation()) {
        currentSense = Mathf.Max(currentSense - senseIncreaseSpeed * Time.deltaTime, 0.0f);
      }

      // Draw and shoot 
      bool currentIsDrawing =
        (input.attack() || (currentDrawingTime > 0f && currentDrawingTime < minDrawingTime)) && !isShooting && !isDizzy && !isDashing() && !input.meditation();
      // Shoot if string is released
      if(isDrawing && !currentIsDrawing && !isDizzy) {
        StartCoroutine(shoot());
      }
      // Set drawing time
      if(currentIsDrawing) {
        currentDrawingTime += Time.deltaTime;
        // render indicator
        if(trailIndicator != null) {
          float velocity = Mathf.Lerp(minArrowSpeed, maxArrowSpeed, getDrawIntensity());
          trailIndicator.isShow = true;
          trailIndicator.initVelocity = new Vector2(velocity, 0.0f);
        }
      } else {
        currentDrawingTime = 0.0f;
        if(trailIndicator != null) {
          trailIndicator.isShow = false;
        }
      }
      isDrawing = currentIsDrawing;

      // Movement
      // vertical movement
      bool isCrouching = false;
      if(groundSensor.isStay() && !isDizzy && !input.meditation()) {
        remainingDash = 1;
        if(input.getVertical() < -0.1f) {
          // jump down
          if(input.jump() && isAllOneSide(groundSensor.getTriggerGos())) {
            StartCoroutine(jumpDown(groundSensor.getTriggerGos()));
          } else if(isDrawing) {
            // crouch drawing

          } else {
            // disable crouch
            // isCrouching = true;
          }
        } else if(input.jump()) {
          // jump up
          rb2d.velocity = new Vector2(rb2d.velocity.x, 0.0f);
          appliedForce = new Vector2(0.0f, jumpForce);
        }
      }

      // Relay jump
      if(!isDizzy
        && !groundSensor.isStay()
        && canRelay()
        && input.jump()) {
        rb2d.velocity = Vector2.zero;
        //rb2d.AddForce(new Vector2(0.0f, jumpForce));
        appliedForce = new Vector2(0.0f, doubleJumpForce);
        // effect
        GameObject doubleJumpEffect = Instantiate(doubleJumpEffectPrefab);
        doubleJumpEffect.transform.position = doubleJumpEffectPoint.position;
        Common.Utils.setRelativeRenderLayer(
          spriteRenderer, doubleJumpEffect.GetComponentInChildren<SpriteRenderer>(), 1);
        // Reset dash
        remainingDash = 1;
      }

      if (canRelay()) {
        Time.timeScale = timeScaleInRelay;
      } else if (isDizzy) {
        Time.timeScale = timeScaleInDizzy;
      } else if (dashknowbackTimeslowRemaining > 0f) {
        Time.timeScale = dashKnowbackTimeslowScale;
        dashknowbackTimeslowRemaining -= Time.deltaTime;
      } else {
        Time.timeScale = 1.0f;
      }

      // Cliff jump
      float topFallingSpeed = maxFallingSpeed;
      isCliffSliding = false;
      if(!groundSensor.isStay() && !isDizzy) {
        bool desiredFacingOrientation = getOrientation() * desiredSpeed > 0.0f;
        if(canDetectCliff && frontSensor.isStay() && desiredFacingOrientation) {
          remainingDash = 1;
          if(input.jump() && canCliffJump) {
            rb2d.velocity = Vector2.zero;
            appliedForce = new Vector2(-Mathf.Sign(desiredSpeed) * cliffJumpForce, cliffJumpForce * 0.8f);
            // cliff jump effect
            GameObject cliffJumpEffect = GameObject.Instantiate(cliffJumpEffectPrefab);
            cliffJumpEffect.transform.position = backwardCliffJumpEffectPoint.position;
            cliffJumpEffect.transform.localScale = new Vector2(-getOrientation(), 1.0f);
            Common.Utils.setRelativeRenderLayer(
              spriteRenderer, cliffJumpEffect.GetComponentInChildren<SpriteRenderer>(), 1);
          } else {
            topFallingSpeed = cliffSlidingSpeed;
            isCliffSliding = true;
          }
        } else if(canDetectCliff && backSensor.isStay() && desiredFacingOrientation && input.jump() && canCliffJump) {
          remainingDash = 1;
          rb2d.velocity = Vector2.zero;
          appliedForce = new Vector2(Mathf.Sign(desiredSpeed) * cliffJumpForce, cliffJumpForce);
          // cliff jump effect
          GameObject cliffJumpEffect = GameObject.Instantiate(cliffJumpEffectPrefab);
          cliffJumpEffect.transform.position = forwardCliffJumpEffectPoint.position;
          cliffJumpEffect.transform.localScale = new Vector2(getOrientation(), 1.0f);
          Common.Utils.setRelativeRenderLayer(
            spriteRenderer, cliffJumpEffect.GetComponentInChildren<SpriteRenderer>(), 1);
        }
      }

      // Dash
      if(isDizzy && isDashing()) {
        exitDash();
      }
      if(!isDizzy && !input.meditation()) {
        if(isDashing()) {
          dashRemainingTime -= Time.deltaTime;
          if(dashRemainingTime < 0.0f) {
            // exit dash
            exitDash();
          }
        } else if(input.dash() && canDash(isCliffSliding)) {
          // enter dash
          rb2d.velocity = new Vector2(Mathf.Sign(getOrientation()) * dashSpeed, 0.0f);
          gravityScale = rb2d.gravityScale;
          rb2d.gravityScale = 0.0f;
          dashRemainingTime = dashTime;
          remainingDash = 0;
          dashCooldownRemaining = dashCooldown;
          if (dashParticle != null) {
            ParticleSystem.EmissionModule emission = dashParticle.emission;
            emission.enabled = true;
          }
          if (dashKnockbackContactDamage != null) {
            dashKnockbackContactDamage.enabled = true;
          }
        }
      }

      // meditation
      isMeditation = false;
      if(!isDizzy && groundSensor.isStay() && input.meditation()) {
        currentSense = Mathf.Min(maxSenseAdd, currentSense + senseIncreaseSpeed * Time.deltaTime);
        isMeditation = true;
      }

      // smash
      if(!isDizzy &&
        !isLastUpdateOnGround &&
        groundSensor.isStay() &&
        rb2d.velocity.y < -smashMinSpeed) {

        ReleaseSmashEffect();

        /*
        GameObject smashEffect = Instantiate(smashEffectPrefab);
        smashEffect.transform.position = smashEffectPoint.position;
        Common.Utils.setRelativeRenderLayer(spriteRenderer, smashEffect.GetComponentInChildren<SpriteRenderer>(), 1);
        */
      }
      isLastUpdateOnGround = groundSensor.isStay();

      // horizontal movement
      /*
      gameObject.transform.parent =
          groundSensor.isStay() ? Utils.getAnyFrom(groundSensor.getTriggerGos()).transform : null;
      */

      // Move with the platform.
      GameObject currentGround = groundSensor.isStay() ? Common.Utils.getAnyFrom(groundSensor.getTriggerGos()) : null;
      if (currentGround != previousParentGameObject) {
        previousParentGameObject = currentGround;
      } else if (currentGround != null){
        // else move with the ground
        Vector3 deltaGroundPosition = currentGround.transform.position - previousParentPosition;
        gameObject.transform.position += new Vector3(deltaGroundPosition.x, deltaGroundPosition.y);
      }
      if (currentGround != null) {
        previousParentPosition = currentGround.transform.position;
      }

      if(dashRemainingTime <= 0.0f) {
        if(!isDizzy && !input.meditation()) {
          if(Mathf.Abs(desiredSpeed) > Mathf.Epsilon
            && (!groundSensor.isStay() || !isDrawing)) {
            rb2d.AddForce(new Vector2(acceleration * desiredSpeed, 0.0f));
            float maxHorizontalSpeed = isCrouching ? maxCrouchSpeed : maxRunningSpeed;
            rb2d.velocity = new Vector2(
              Mathf.Clamp(rb2d.velocity.x, -maxHorizontalSpeed, maxHorizontalSpeed),
              rb2d.velocity.y);
          } else {
            rb2d.velocity = new Vector2(0.0f, rb2d.velocity.y);
          }
          // else, keep same speed
        } else {
          // dizzy, damp on horizontal speed
          rb2d.velocity =
            new Vector2(rb2d.velocity.x * 0.8f, rb2d.velocity.y);
        }
      }

      // Apply force
      rb2d.AddForce(appliedForce);

      // Update facing
      if(Mathf.Abs(desiredSpeed) > Mathf.Epsilon
        && !isDizzy
        && !input.meditation()
        && dashRemainingTime < Mathf.Epsilon) {
        float parentLossyScale = gameObject.transform.parent != null
            ? gameObject.transform.parent.lossyScale.x : 1.0f;
        if(desiredSpeed * parentLossyScale > 0.0f) {
          transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        if(desiredSpeed * parentLossyScale < 0.0f) {
          transform.localScale = new Vector3(
            -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
      }

      // limit falling speed
      if(rb2d.velocity.y < 0.0f) {
        rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Max(rb2d.velocity.y, -topFallingSpeed));
      }

      // better jump
      if (rb2d.velocity.y < 0) {
        rb2d.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1.0f) * Time.deltaTime;
      } else if (rb2d.velocity.y > .0f && !input.jumpHigher()) {
        rb2d.velocity += Vector2.up * Physics2D.gravity.y * (lowJumMultiplier - 1.0f) * Time.deltaTime;
      }

      // Update head collider
      /*
      if(headCollider != null) {
        headCollider.enabled = !isCrouching;
      }
      */

      // Update animation
      animator.SetBool(GROUNDED, groundSensor.isStay());
      animator.SetFloat(H_SPEED, Mathf.Abs(rb2d.velocity.x));
      animator.SetFloat(V_SPEED, rb2d.velocity.y);
      animator.SetBool(DRAWING, isDrawing);
      animator.SetBool(DIZZY, isDizzy);
      animator.SetBool(CROUCH, isCrouching);
      animator.SetBool(CLIFF_SLIDING, isCliffSliding && rb2d.velocity.y < Mathf.Epsilon);
      animator.SetBool(DASHING, isDashing());
    }

    private void ReleaseSmashEffect() {
      if (smashParticleSystem != null) {
        smashParticleSystem.Play();
      }
    }

    public void damage(DamageInfo damageInfo) {
      if(Time.time - lastGetDamagedTime < immutableTime) {
        return;
      }

      lastGetDamagedTime = Time.time;
      Bullets.Utils.ApplyRepel(damageInfo, rb2d);
      currentHealth -= damageInfo.damage;
      if(currentHealth <= 0) {
        // Die
        Common.SceneConfig.getSceneConfig().getProgressManager().Load();
      } else {
        // Dizzy
        cinemachineImpulseSource.GenerateImpulse();
        StartCoroutine(dizzy());
        // Effect
        damageEffectAnimator.SetTrigger("onDamage");
        damageEffectParticleSystem.Emit(30);
      }
    }

    public float getDrawIntensity() {
      return currentDrawingTime / maxDrawingTime;
    }

    public float getOrientation() {
      return transform.lossyScale.x > 0.0f ? 1.0f : -1.0f;
    }

    public bool isDashing() {
      return dashRemainingTime > 0.0f;
    }

    public bool canDash(bool isCliffSliding) {
      return remainingDash > 0 && dashCooldownRemaining <= 0.0f && !isCliffSliding;
    }

    public void registerRelayPoint(RelayPoint relayPoint) {
      activeRelayPoints.Add(relayPoint);
    }

    public void unregisterRelayPoint(RelayPoint relayPoint) {
      activeRelayPoints.Remove(relayPoint);
    }

    public void generateDust() {
      dustParticleSystem.Play(false);
    }

    public bool isNearestRelayPoint(RelayPoint relayPoint) {
      return relayPoint == nearestRelayPoint;
    }

    private bool canRelay() {
      return !isDizzy
        && !groundSensor.isStay()
        && supportRelay
        && nearestRelayPoint != null
        && getRelayPointDistanceSqrt(nearestRelayPoint) < relayEffectDistance * relayEffectDistance;
    }

    private void updateNearestRelayPoint() {
      nearestRelayPoint = null;
      float nearestPointDistanceSqr = Mathf.Infinity;
      foreach (RelayPoint relayPoint in activeRelayPoints) {
        float currentDistance = getRelayPointDistanceSqrt(relayPoint);
        if (currentDistance < nearestPointDistanceSqr) {
          nearestRelayPoint = relayPoint;
          nearestPointDistanceSqr = currentDistance;
        }
      }
    }

    private float getRelayPointDistanceSqrt(RelayPoint relayPoint) {
      return Common.Utils.toVector2(transform.position - relayPoint.transform.position).SqrMagnitude();
    }

    private bool isAllOneSide(HashSet<GameObject> gameObjects) {
      foreach(GameObject gameObject in gameObjects) {
        if(!gameObject.CompareTag(Common.Tags.ONESIDE)) {
          return false;
        }
      }
      return true;
    }

    private void exitDash(float delayStateChange = 0f) {
      dashRemainingTime = Mathf.Min(delayStateChange, dashRemainingTime);
      rb2d.gravityScale = gravityScale;

      if (dashParticle != null) {
        ParticleSystem.EmissionModule emission = dashParticle.emission;
        emission.enabled = false;
      }

      if (dashKnockbackContactDamage != null) {
        dashKnockbackContactDamage.enabled = false;
      }
    }

    private void onDashKnockbackEvent() {
      if (dashKnockbackParticle != null) {
        ParticleSystem.EmissionModule emission = dashKnockbackParticle.emission;
        dashKnockbackParticle.Emit(dashKnowbackParticleNumber);
      }
      cinemachineImpulseSource.GenerateImpulse();
      dashknowbackTimeslowRemaining = dashKnowbackTimeslowDuration;
      // delay exiting dash animiation to have slow motion on dash action
      exitDash(.1f);
    }

    private IEnumerator shoot() {
      Debug.Assert(arrowPrefab != null, "Arrow prefab is not set");
      Debug.Assert(shootPoint != null, "Shoot point is not set");

      GameObject arrow = Instantiate(arrowPrefab, shootPoint.position + Vector3.forward * 0.1f, shootPoint.rotation);
      float drawingRatio =
        Mathf.Clamp(currentDrawingTime, 0.0f, maxDrawingTime) / maxDrawingTime;
      bool isStrongArrow = drawingRatio > strongArrowDrawingRatio;

      // Set arrow 
      TrailRenderer trailRenderer = arrow.GetComponentInChildren<TrailRenderer>();
      trailRenderer.time = Mathf.Lerp(minTrailTime, maxTrailTime, getDrawIntensity());
      ArrowCarrier arrowCarrier = arrow.GetComponent<ArrowCarrier>();

      arrowCarrier.SetIsShellBreaking(isStrongArrow);
      arrowCarrier.repelIntensive = isStrongArrow ? maxRepelForce : quickRepelForce;
      arrowCarrier.fire(
        new Vector2(transform.lossyScale.x > 0.0f ? maxArrowSpeed : -maxArrowSpeed, 0.0f),
        isStrongArrow ? maxArrowLifetime : quickArrowLifetime,
        gameObject.tag);
      isShooting = true;
      yield return new WaitForSeconds(shootingCd);
      isShooting = false;
    }

    private IEnumerator jumpDown(HashSet<GameObject> onesideGos) {
      List<GameObject> disabledColliders = new List<GameObject>(onesideGos);
      foreach(GameObject gameObject in disabledColliders) {
        Collider2D collider = gameObject.GetComponent<Collider2D>();
        collider.enabled = false;
      }
      yield return new WaitForSeconds(1.0f);
      disabledColliders.ForEach(gameObject => {
        Collider2D collider = gameObject.GetComponent<Collider2D>();
        collider.enabled = true;
      });
    }

    private IEnumerator dizzy() {
      isDizzy = true;
      yield return new WaitForSeconds(dizzyTime);
      isDizzy = false;
      damageEffectAnimator.ResetTrigger("onDamage");
    }

    public void AssignDustTexture(DustTexture dustTexture) {
      dustTexture.ApplyTexture(dustParticleSystem);
      if (smashParticleSystem != null) {
        foreach (var particleSystem in smashParticleSystem.gameObject.GetComponentsInChildren<ParticleSystem>()) {
          dustTexture.ApplyTexture(particleSystem);
        }
      }
    }
  }
}

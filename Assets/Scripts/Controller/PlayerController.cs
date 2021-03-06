﻿using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

using Catsland.Scripts.Bullets;
using Catsland.Scripts.Common;
using Catsland.Scripts.Misc;
using Catsland.Scripts.Physics;
using Catsland.Scripts.Fx;
using IDamageInterceptor = Catsland.Scripts.Bullets.IDamageInterceptor;

namespace Catsland.Scripts.Controller {

  [RequireComponent(typeof(IInput))]
  [RequireComponent(typeof(Animator))]
  public class PlayerController : MonoBehaviour, DustTexture.DustTextureAssignee, IDamageInterceptor {

    private const string TIME_SCALE_CONFIG_NAME = "PlayerController";
    private const int TIME_SCALE_CONFIG_PRIORITY = 10;
    
    [System.Serializable]
    public class PlayerShakeInfo {
      public float duration;
      public float strength;
    }


    public float timeScaleChangeSpeed = 1f;
    private GhostSprite ghostSprite;
    public float timeSlowPitch = .5f;

    public GameObject upperBodyGo;
    public TimeScaleConfig normalTimeScale;

    // Locomoation
    [Header("Run")]
    public float maxRunningSpeed = 1.0f;
    public float maxCrouchSpeed = 0.5f;
    public float acceleration = 1.0f;

    private Sound.Sound footstepSound;
    private GameObject lastGroundObject;

    [Header("Jump")]
    public float jumpForce = 5.0f;
    public float jumpVelocity = 2f;
    public float doubleJumpForce = 10.0f;
    public float doubleJumpVelocity = 2f;
    public float cliffInwardJumpForce = 480.0f;
    public float cliffOutwardJumpForce = 800.0f;
    public float cliffInwardJumpVelocity = 1f;
    public float cliffOutwardJumpVelocity = 2f;
    public float cliffOutwardJumpDegree = 45f;
    public float cliffInwardJumpDegree = 80f;
    public float maxFallingSpeed = 5.0f;
    public float cliffSlidingSpeed = 1.0f;
    public float fallMultiplier = 2.5f;
    public float lowJumMultiplier = 2f;
    public ParticleSystem smashParticleSystem;
    public ParticleSystem jumpUpDustParticle;
    public Sound.Sound jumpSound;
    public Sound.Sound landSoftSound;
    public AudioSource jumpAudioSource;
    public float leaveGroundGracePeroid = .1f;
    public bool supportTimeslow = true;

    public float cliffJumpGravatyScale = 0.5f;
    public float cliffJumpTime = 0.5f;
    private float cliffJumpRemaining = 0f;

    [Header("Dash")]
    public bool supportDash = true;
    public float dashSpeed = 3.0f;
    public float dashTime = 0.6f;
    public float dashCooldown = 1.0f;
    public Vector2 dashCameraShakeScale = new Vector2(0.5f, 0.0f);
    // If true, player orientation will affect the sign of x of dashCameraShakeScale.
    public bool dashCameraShakeApplyOrientationImpact = true;
    public int dashKnowbackParticleNumber = 120;
    //public float dashKnowbackTimeslowDuration = 2f;
    public TimeScaleConfig dashKnockbackTimeScale;
    public float dashKnowbackRepelSpeed = 2f;
    public AudioSource dashAudioSource;
    public Sound.Sound dashSound;
    private float dashknowbackTimeslowRemaining = 0f;

    public Sound.Sound dashKnockbackSuccessSound;

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
    public TimeScaleConfig relayTimeScale;

    public float relayKickIntensity = 200f;
    public TriggerBasedSensor replyJumpSensor;
    public GameObject relayKickEffectPrefab;
    public Transform relayKickEffectPosition;

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
    private float lastOnGroundTimestamp = 0f;

    // Attack
    [Header("Shoot")]
    public Party.WeaponPartyConfig weaponPartyConfig;
    public bool canStrongShoot = false;
    public float maxArrowSpeed = 15.0f;
    public float strongArrowSpeed = 30f;
    public float minArrowSpeed = 5.0f;
    public float maxDirectionAngle = 30f;
    public float shootDirectionAngle = 0f;
    public float maxArrowDirectionSpreadAngle = 10f;
    public float maxBowHeat = 10;
    public float bowHeatIncreasePerShoot = 1f;
    public float bowHeatCooldownPerSecond = 2f;
    public ParticleSystem shootEffectParticle;
    public ControllerVibrationStackEffectConfig normalShootVibrateConfig;
    public ControllerVibrationStackEffectConfig timeSlowDrawVibrateConfig;
    // Currently only the strength is used.
    public PlayerShakeInfo normalShootShakeConfig;

    private float currentBowHeat = 0f;

    public ParticleSystem quickDrawParticle;

    public TimeScaleConfig jumpAndAimTimeScale;
    public float autoAimDetectRadius = 8f;
    public float autoAimCoverAngle = 10f;

    public AudioSource shootAudioSource;
    public Sound.Sound quickShotSound;
    public Sound.Sound strongShotSound;

    public float quickArrowLifetime = 1.0f;
    public float maxArrowLifetime = 3.0f;

    public float maxRepelForce = 800f;
    public float quickRepelForce = 300f;
    public float minDrawingTime = 0.5f;
    public float maxDrawingTime = 2.0f;
    public float strongArrowDrawingRatio = 0.9f;
    public float minTrailTime = 1f;
    public float maxTrailTime = 3f;

    public int rapidShootLimit = 3;
    public int rapidShootRemain;

    public float shootingCooldownSeconds = .5f;
    private float shootingCooldownRemainSeconds = 0f;

    // Experimental
    public bool isRopeArrow = true;
    public GameObject ropeArrowPrefab;
    public float ropeForceApplyTime = 0.4f;
    public Transform arrowRopeAttachPoint;
    private bool hasRopeLinked = false;

    private bool isDrawing = false;
    private bool isShooting = false;
    // TODO: hide after debug
    public float currentDrawingTime = 0.0f;
    private HashSet<RelayPoint> activeRelayPoints = new HashSet<RelayPoint>();
    private RelayPoint nearestRelayPoint;

    // Health
    [Header("Health")]
    public int maxHealth = 3;
    public int currentHealth;
    public float repelHorizontalVelocity = 1f;
    public float repelVerticalVelocity = 1f;
    public float dizzyTime = 1.0f;
    public TimeScaleConfig dizzyTimeScale;
    public float immutableTime = 0.5f;
    public int score = 0;
    public AudioSource damageAudioSource;
    public Sound.Sound damageSound;
    public ControllerVibrationStackEffectConfig takeDamageVibrationConfig;


    private float ropeForceRemaining = 0f;

    private bool isDizzy = false;
    private float lastGetDamagedTime = 0.0f;

    public Timing timing;

    private bool isDead = false;

    // References
    public GameObject groundSensorGO;
    public GameObject headSenserGo;
    public GameObject frontSensorGo;
    public GameObject backSensorGo;
    public GameObject arrowPrefab;
    public GameObject strongArrowPrefab;
    public Transform shootPoint;
    public TrailIndicator trailIndicator;
    public GameObject cliffJumpEffectPrefab;
    public Transform smashEffectPoint;
    public GameObject smashEffectPrefab;
    public Transform forwardCliffJumpEffectPoint;
    public Transform backwardCliffJumpEffectPoint;
    public GameObject doubleJumpEffectPrefab;
    public Transform doubleJumpEffectPoint;
    public ParticleSystem damageEffectParticleSystem;
    private AudioSource audioSource;

    [Header("Experiment")]
    public bool fireArrow;
    // Whether to limit the shooting direction to 0, +_45.
    public bool discreteShootingDirection = false;
    public float discreteShootingVerticleThreshold = 0.1f;
    public bool smoothShootingDirection = false;
    public float smoothShootingDirectionSpeed = 90f;
    public GameObject autoAimUiPrefab;
    private GameObject autoAimUiGo;

    public bool enableAutoAim = true;
    private GameObject autoAimGo;
    private ControllerVibrationManager controllerVibrationManager;

    public Vector3 footPosition {
      get {
        return groundSensorGO.transform.position;
      }
    }


    private float totalApplyForce = 0f;

    private ISensor groundSensor;
    private ISensor backSensor;
    private ISensor frontSensor;
    private IInput input;
    private Rigidbody2D rb2d;
    private Animator animator;
    private BoxCollider2D headCollider;
    private SpriteRenderer spriteRenderer;
    private CinemachineImpulseSource cinemachineImpulseSource;
    private TimeScaleController timeScaleController;

    private GameObject previousParentGameObject;
    private Vector3 previousParentPosition;
    public ParticleSystem dustParticleSystem;
    public float dustGenerationIntervalInS = .1f;
    private float dustGenerationInternalRemainInS = 0f;

    // Animation
    private const string H_SPEED = "HSpeed";
    private const string V_SPEED = "VSpeed";
    private const string GROUNDED = "Grounded";
    private const string DRAWING = "Drawing";
    private const string DIZZY = "Dizzy";
    private const string CROUCH = "Crouch";
    private const string CLIFF_SLIDING = "CliffSliding";
    private const string DASHING = "Dashing";
    private const string DIRECTION = "UpperBodyDirection";
    private const string IS_IN_DRAWING_CYCLE = "IsInDrawingCycle";

    private const string DRAWING_STATE_NAME = "Drawing";
    private const string PREPARING_STATE_NAME = "Upper_Preparing";
    private const string FAST_RELOAD_STATE_NAME = "Upper_FastReload";
    private static readonly string[] NON_DRAWING_CYCLE_STATES = { "Empty", "Upper_ShootDone" };

    private const float DEFAULT_PHYSICS_TIMESTAMP = .02f;

    [System.Serializable]
    public class Snapshot {
      public int currentHp;
      public int maxHp;
      public bool canRelayJump;
      public bool canStrongShoot;
      public bool canDash;
      public bool canCliffJump;
      public bool canTimeslow;
    }


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
      audioSource = GetComponent<AudioSource>();
      ghostSprite = GetComponent<GhostSprite>();
      timeScaleController = GameObject.FindGameObjectWithTag(Tags.TIME_SCALE_CONTROLLER)
        ?.GetComponent<TimeScaleController>();

      rapidShootRemain = rapidShootLimit;
      controllerVibrationManager = FindObjectOfType<ControllerVibrationManager>();
    }

    public void Awake() {
      currentHealth = maxHealth;
      dashKnockbackContactDamage.enabled = false;
      dashKnockbackContactDamage.onHitEvent = onDashKnockbackEvent;
      dashKnockbackContactDamage.onDamageFeedback = onDashKnockbackFeedbackEvent;
    }

    public void Update() {
      float desiredSpeed = input.getHorizontal();
      float currentVerticleVolocity = rb2d.velocity.y;
      Vector2 appliedForce = Vector2.zero;
      Vector2 appliedVelocity = Vector2.zero;

      if (ropeForceRemaining > 0f) {
        ropeForceRemaining -= Time.deltaTime;
      }

      // Relay point update
      updateNearestRelayPoint();

      // Cooldown
      if (dashCooldownRemaining > 0.0f) {
        dashCooldownRemaining -= Time.deltaTime;
      }
      if (!input.meditation()) {
        currentSense = Mathf.Max(currentSense - senseIncreaseSpeed * Time.deltaTime, 0.0f);
      }
      if (currentBowHeat > 0f) {
        currentBowHeat -= Time.deltaTime * bowHeatCooldownPerSecond;
      }
      if (shootingCooldownRemainSeconds > 0f) {
        shootingCooldownRemainSeconds -= Time.deltaTime;
      } else {
        rapidShootRemain = rapidShootLimit;
      }
      // Draw and shoot 
      AnimatorStateInfo upperBodySpriteLayerState = animator.GetCurrentAnimatorStateInfo(2);
      bool currentIsDrawing =
        (input.attack() || (currentDrawingTime > 0f && currentDrawingTime < minDrawingTime)
          || upperBodySpriteLayerState.IsName(PREPARING_STATE_NAME)
          || upperBodySpriteLayerState.IsName(FAST_RELOAD_STATE_NAME))
        && !isShooting && !isDizzy && !isDashing() && !input.meditation() && !isCliffJumping() && !isCliffSliding
        && (shootingCooldownRemainSeconds < Mathf.Epsilon || rapidShootRemain > 0);

      // Shoot if string is released
      if (isDrawing && !currentIsDrawing && !isDizzy && !isCliffJumping() && !isCliffSliding) {
        StartCoroutine(shoot());
      }
      // clear up shoot direction.
      // Timeslow will keep the direction
      if (!isInDrawingCycle() && !input.timeSlow()) {
        // Immediately reset to 0 for non-drawing statuses.
        shootDirectionAngle = 0f;
      }
      // Set drawing time
      if (currentIsDrawing) {
        // Do not accumulate time in preparing state: PREPARING_STATE_NAME and FAST_RELOAD_STATE_NAME
        if (upperBodySpriteLayerState.IsName(DRAWING_STATE_NAME)) {
          // Use unscaled time so the player gains extra time during time slow.
          currentDrawingTime += Time.unscaledDeltaTime;
        }
        // vibrate in timeslow mode
        if (input.timeSlow()) {
          controllerVibrationManager.RegisterConfig(timeSlowDrawVibrateConfig, true);
        }

        // render indicator
        if (trailIndicator != null) {
          if (input.timeSlow()) {
            float velocity = Mathf.Lerp(minArrowSpeed, maxArrowSpeed, getDrawIntensity());
            trailIndicator.isShow = true;
            trailIndicator.initVelocity = new Vector2(velocity, 0.0f);
          } else {
            trailIndicator.isShow = false;
          }
        }
        // shooting direction
        // 1. raw direction.
        Vector2 desiredDirection = input.timeSlow()
          ? getShootingDirection()
          : new Vector2(getOrientation(), 0f);

        // If not in time slow and not indicate shooting direction, use front.
        // Otherwise keep previous direction.
        if (desiredDirection.sqrMagnitude < Mathf.Epsilon && !input.timeSlow()) {
          desiredDirection = new Vector2(getOrientation(), 0f);
        }

        // 2. Adjust direction if autoAim is enabled.
        desiredDirection = autoAim(desiredDirection);

        // 3. Direction to rotation angle
        float desiredDirectionAngle;
        // In timeslow mode, if not direction input is given, keep current direction.
        if (desiredDirection.sqrMagnitude < Mathf.Epsilon && input.timeSlow()) {
          desiredDirectionAngle = shootDirectionAngle;
        } else {
          if (desiredDirection.x > 0f) {
            desiredDirectionAngle = Mathf.Clamp(Vector2.SignedAngle(Vector2.right, desiredDirection), -maxDirectionAngle, maxDirectionAngle);
          } else {
            desiredDirectionAngle = Mathf.Clamp(Vector2.SignedAngle(desiredDirection, Vector2.left), -maxDirectionAngle, maxDirectionAngle);
          }
        }
        // 4. Smooth angle.
        if (smoothShootingDirection) {
          float absoluateDelta = Mathf.Abs(desiredDirectionAngle - shootDirectionAngle);
          float lerpProgress = absoluateDelta < Mathf.Epsilon
            ? 1f
            : Mathf.Clamp01(Time.unscaledDeltaTime * smoothShootingDirectionSpeed);
          desiredDirectionAngle = Mathf.LerpAngle(shootDirectionAngle, desiredDirectionAngle, lerpProgress);
        }
        shootDirectionAngle = desiredDirectionAngle;
      } else {
        currentDrawingTime = 0.0f;
        if (trailIndicator != null) {
          trailIndicator.isShow = false;
        }
      }
      isDrawing = currentIsDrawing;
      // reset autoAim ui if not drawing
      if (!isDrawing) {
        autoAimGo = null;
      }
      UpdateAutoAimUi();

      // Movement
      // vertical movement
      bool isCrouching = false;
      if (canNormalJump()) {
        remainingDash = 1;
        // The following code enable one-side platform jump down. However, it creates the 
        // odds that if player unintentionally press down and jump, the player cannot jump up.
        /*
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
        */
        if (input.jump()) {
          rb2d.velocity = new Vector2(rb2d.velocity.x, 0.0f);
          appliedForce = new Vector2(0.0f, jumpForce);
          appliedVelocity = new Vector2(rb2d.velocity.x, jumpVelocity);
          // jump up dust.
          generateJumpUpDust();
        }
      }

      // Relay jump
      if (!isDizzy
        && !groundSensor.isStay()
        && canRelay()
        && !isCliffJumping()
        && input.jump()) {
        rb2d.velocity = Vector2.zero;
        //rb2d.AddForce(new Vector2(0.0f, jumpForce));
        appliedForce = new Vector2(0.0f, doubleJumpForce);
        appliedVelocity = new Vector2(0f, doubleJumpVelocity);

        // Reset dash
        remainingDash = 1;

        // For jump on object
        if (isRelaySensorTriggered()) {
          foreach (GameObject kicked in replyJumpSensor.getTriggerGos()) {
            kicked.SendMessage(
              MessageNames.DAMAGE_FUNCTION,
              new DamageInfo(
                /* damage= */0, replyJumpSensor.transform.position, Vector2.down, relayKickIntensity,
                /* isSmashAttack= */false,
                /* isDash= */false,
                /* isKick= */true),
              SendMessageOptions.DontRequireReceiver);

            // Effect
            GameObject effect = Instantiate(relayKickEffectPrefab);
            effect.transform.position = new Vector3(relayKickEffectPosition.position.x, relayKickEffectPosition.position.y, AxisZ.RELAY_KICK_EFFECT);
            effect.GetComponent<ParticleSystem>()?.Emit(60);
            Destroy(effect, 1.0f);
          }
        } else {
          // Relay point effect
          nearestRelayPoint?.Kicked();

          // effect
          GameObject doubleJumpEffect = Instantiate(doubleJumpEffectPrefab);
          doubleJumpEffect.transform.position = doubleJumpEffectPoint.position;
          // TODO: clean up
          Common.Utils.setRelativeRenderLayer(
            spriteRenderer, doubleJumpEffect.GetComponentInChildren<SpriteRenderer>(), 1);
        }
      }

      bool enableGhost = !groundSensor.isStay();
      // TODO: make this parameters
      float ghostLifetime = .15f;
      float ghostInterval = .01f;
      if (canRelay() && wantTimeSlow() && canTimeSlow()) {
        setTimeScale(relayTimeScale);
      } else if (isDizzy) {
        setTimeScale(dizzyTimeScale);
      } else if (dashknowbackTimeslowRemaining > 0f) {
        setTimeScale(dashKnockbackTimeScale);
        dashknowbackTimeslowRemaining -= Time.deltaTime;
        if (dashknowbackTimeslowRemaining <= 0f) {
          exitDashKnockback();
        }
      } else if (wantTimeSlow() && canTimeSlow() && isInDrawingCycle() && !groundSensor.isStay()) {
        setTimeScale(jumpAndAimTimeScale, /* Animator unscaled */ true);
        ghostLifetime = .5f;
        ghostInterval = .08f;
      } else {
        setTimeScale(normalTimeScale);
      }
      // Ghost Fx
      ghostSprite.emit = enableGhost;
      ghostSprite.ghostLifetimeSecond = ghostLifetime;
      ghostSprite.spawnInterval = ghostInterval;

      float preCliffJumpRemaining = cliffJumpRemaining;
      if (cliffJumpRemaining > 0.0f) {
        cliffJumpRemaining -= Time.deltaTime;
      }
      // exit cliff jump
      if (preCliffJumpRemaining > 0f && cliffJumpRemaining <= 0f) {
        rb2d.gravityScale = gravityScale;
      }
      // Cliff jump / sliding
      float topFallingSpeed = maxFallingSpeed;
      isCliffSliding = false;
      if (!groundSensor.isStay() && !isDizzy && !isCliffJumping()) {
        // cliff jump
        bool canJumpFront = backSensor.isStay();
        bool canJumpBack = frontSensor.isStay();
        if ((canJumpFront || canJumpBack) && canCliffJump && input.jump()) {
          bool canJumpXPositive = getOrientation() > 0f ? canJumpFront : canJumpBack;
          bool canJumpXNegative = getOrientation() > 0f ? canJumpBack : canJumpFront;
          float jumpOrientation = 0f;
          if (canJumpXPositive && canJumpXNegative) {
            jumpOrientation = Mathf.Sign(desiredSpeed);
          } else {
            jumpOrientation = canJumpXPositive ? 1f : -1f;
          }
          remainingDash = 1;
          rb2d.velocity = Vector2.zero;
          float degree = Mathf.Deg2Rad * ((desiredSpeed * jumpOrientation > 0f) ? cliffOutwardJumpDegree : cliffInwardJumpDegree);
          float cliffJumpForce = (desiredSpeed * jumpOrientation > 0f) ? cliffOutwardJumpForce : cliffInwardJumpForce;
          float cliffJumpVelocity = (desiredSpeed * jumpOrientation > 0f) ? cliffOutwardJumpVelocity : cliffInwardJumpVelocity;
          appliedForce = new Vector2(jumpOrientation * cliffJumpForce * Mathf.Sin(degree), cliffJumpForce * Mathf.Cos(degree));
          appliedVelocity = new Vector2(jumpOrientation * cliffJumpVelocity * Mathf.Sin(degree), cliffJumpVelocity * Mathf.Cos(degree));
          cliffJumpRemaining = cliffJumpTime;

          // cliff jump effect
          GameObject cliffJumpEffect = GameObject.Instantiate(cliffJumpEffectPrefab);
          rb2d.gravityScale = cliffJumpGravatyScale;

          if (getOrientation() * jumpOrientation < 0f) {
            cliffJumpEffect.transform.position = backwardCliffJumpEffectPoint.position;
            cliffJumpEffect.transform.localScale = new Vector2(-getOrientation(), 1.0f);
          } else {
            cliffJumpEffect.transform.position = forwardCliffJumpEffectPoint.position;
            cliffJumpEffect.transform.localScale = new Vector2(getOrientation(), 1.0f);
          }
          Common.Utils.setRelativeRenderLayer(
            spriteRenderer, cliffJumpEffect.GetComponentInChildren<SpriteRenderer>(), 1);
        } else if ((canJumpBack || canJumpFront) && canCliffJump && getOrientation() * desiredSpeed > 0f) {
          topFallingSpeed = cliffSlidingSpeed;
          isCliffSliding = true;
        }
      }

      // Dash
      if (isDizzy && isDashing()) {
        exitDash();
      }
      if (!isDizzy && !input.meditation() && !isCliffJumping()) {
        if (isDashing()) {
          dashRemainingTime -= Time.deltaTime;
          if (dashRemainingTime < 0.0f) {
            // exit dash
            exitDash();
          }
        } else if (input.dash() && canDash(isCliffSliding)) {
          // enter dash

          dashSound?.Play(dashAudioSource);
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
            dashKnockbackContactDamage.repelDirection = new Vector2(getOrientation(), 0f);
            dashKnockbackContactDamage.enabled = true;
          }
        }
      }

      // meditation
      isMeditation = false;
      if (!isDizzy && groundSensor.isStay() && input.meditation()) {
        currentSense = Mathf.Min(maxSenseAdd, currentSense + senseIncreaseSpeed * Time.deltaTime);
        isMeditation = true;
      }

      // smash
      if (!isDizzy &&
        !isLastUpdateOnGround &&
        groundSensor.isStay() &&
        rb2d.velocity.y < -smashMinSpeed &&
        !isCliffJumping()) {

        ReleaseSmashEffect();

        /*
        GameObject smashEffect = Instantiate(smashEffectPrefab);
        smashEffect.transform.position = smashEffectPoint.position;
        Common.Utils.setRelativeRenderLayer(spriteRenderer, smashEffect.GetComponentInChildren<SpriteRenderer>(), 1);
        */
      }

      // horizontal movement
      /*
      gameObject.transform.parent =
          groundSensor.isStay() ? Utils.getAnyFrom(groundSensor.getTriggerGos()).transform : null;
      */

      // Move with the platform.
      GameObject currentGround = groundSensor.isStay() ? Common.Utils.getAnyFrom(groundSensor.getTriggerGos()) : null;
      /*
      if (currentGround != previousParentGameObject) {
        previousParentGameObject = currentGround;
      } else if (currentGround != null) {
        // else move with the ground
        Vector3 deltaGroundPosition = currentGround.transform.position - previousParentPosition;
        gameObject.transform.position += new Vector3(deltaGroundPosition.x, deltaGroundPosition.y);
      }
      if (currentGround != null) {
        previousParentPosition = currentGround.transform.position;
      }
      */

      // Can control horzontal speed?
      // Should horitonal stop?
      if (canControlHoritonalSpeed()) {
        if (shouldHoritontalMove()) {
          rb2d.AddForce(new Vector2(acceleration * desiredSpeed, 0.0f));
          float maxHorizontalSpeed = isCrouching ? maxCrouchSpeed : maxRunningSpeed;
          rb2d.velocity = new Vector2(
            Mathf.Clamp(rb2d.velocity.x, -maxHorizontalSpeed, maxHorizontalSpeed),
            rb2d.velocity.y);
        } else {
          rb2d.velocity = new Vector2(0.0f, rb2d.velocity.y);
        }
      } // else keep current speed

      // Apply force
      //rb2d.AddForce(appliedForce);
      if (appliedVelocity.sqrMagnitude > Mathf.Epsilon) {
        rb2d.velocity = appliedVelocity;
      }

      // Update orientation
      if (Mathf.Abs(desiredSpeed) > Mathf.Epsilon
        && !isDizzy
        && !isCliffJumping()
        && !input.meditation()
        && !isDashing()
        && !isDashKnockingBack()) {
        float parentLossyScale = gameObject.transform.parent != null
            ? gameObject.transform.parent.lossyScale.x : 1.0f;
        if (desiredSpeed * parentLossyScale > 0.0f) {
          transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        if (desiredSpeed * parentLossyScale < 0.0f) {
          transform.localScale = new Vector3(
            -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
      }

      // limit falling speed
      if (rb2d.velocity.y < 0.0f) {
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
      animator.SetBool(DASHING, isDashing() || isDashKnockingBack());
      animator.SetFloat(DIRECTION, isInDrawingCycle() ? (shootDirectionAngle + maxDirectionAngle) * .5f / maxDirectionAngle : .5f);
      animator.SetBool(IS_IN_DRAWING_CYCLE, isInDrawingCycle());

      // running dust effect
      if (groundSensor.isStay() && Mathf.Abs(rb2d.velocity.x) > .1f) {
        if (dustGenerationInternalRemainInS < 0f) {
          generateDust();
          dustGenerationInternalRemainInS = dustGenerationIntervalInS;
        } else {
          dustGenerationInternalRemainInS -= Time.deltaTime;
        }
      }
      // land dust effect 
      if (!isLastUpdateOnGround && groundSensor.isStay()) {
        generateDust();
      }

      GameObject currentGroundObject = null;
      if (groundSensor.isStay() && groundSensor.getTriggerGos().Count > 0) {
        currentGroundObject = Common.Utils.getAnyFrom(groundSensor.getTriggerGos());
      }
      if (lastGroundObject != currentGroundObject) {
        Ground ground = currentGroundObject?.GetComponent<Ground>();
        if (ground != null) {
          footstepSound = ground.getFootstepSound();
        } else {
          footstepSound = null;
        }
      }
      lastGroundObject = currentGroundObject;

      // sound effect
      updateFootstepSound();

      debugDrawAimDirection();

      isLastUpdateOnGround = groundSensor.isStay();
      if (isLastUpdateOnGround) {
        lastOnGroundTimestamp = Time.time;
      }
    }

    private Vector2 getShootingDirection() {
      if (discreteShootingDirection) {
        if (Mathf.Abs(input.getVertical()) > discreteShootingVerticleThreshold) {
          return new Vector2(input.getHorizontal(), Mathf.Sign(input.getVertical()) * Mathf.Abs(input.getHorizontal()));
        }
        return new Vector2(input.getHorizontal(), 0f);
      }
      return new Vector2(input.getHorizontal(), input.getVertical());
    }

    private void FixedUpdate() {
      GameObject currentGround = groundSensor.isStay() ? Common.Utils.getAnyFrom(groundSensor.getTriggerGos()) : null;
      if (currentGround != previousParentGameObject) {
        previousParentGameObject = currentGround;
      } else if (currentGround != null) {
        // else move with the ground
        Vector3 deltaGroundPosition = currentGround.transform.position - previousParentPosition;
        gameObject.transform.position += new Vector3(deltaGroundPosition.x, deltaGroundPosition.y);
      }
      if (currentGround != null) {
        previousParentPosition = currentGround.transform.position;
      }
    }

    private bool canControlHoritonalSpeed() => !isDashing()
      && !isDashKnockingBack()
      && !isDizzy
      && !input.meditation()
      && !isCliffJumping()
      && !isApplyingRopeForce();

    // Precondition: canControlHoritonalSpeed() == true.
    private bool shouldHoritontalMove() {
      if (Mathf.Abs(input.getHorizontal()) < Mathf.Epsilon) {
        return false;
      }
      if (groundSensor.isStay()) {
        return !input.timeSlow() && !isInDrawingCycle();
      } else {
        // can always control in the air.
        return true;
      }
    }

    private bool canNormalJump()
      => (groundSensor.isStay() || (Time.time - lastOnGroundTimestamp) < leaveGroundGracePeroid)
        && !isDizzy && !input.meditation() && !isCliffJumping();

    private bool isApplyingRopeForce() => ropeForceRemaining > 0f;

    private bool wantTimeSlow() {
      return input.timeSlow();
    }

    private bool canTimeSlow() {
      return supportTimeslow;
    }

    public void applyRopeForce(Vector2 force) {
      ropeForceRemaining = ropeForceApplyTime;
    }

    private void ReleaseSmashEffect() {
      // TODO: improve the particle
      /*
      if (smashParticleSystem != null) {
        smashParticleSystem.Play();
      }
      */
      landSoftSound?.Play(jumpAudioSource);
    }

    private bool isInDrawingCycle() {
      AnimatorStateInfo upperBodySpriteLayerState = animator.GetCurrentAnimatorStateInfo(2);
      return 
        !NON_DRAWING_CYCLE_STATES.Any(state => upperBodySpriteLayerState.IsName(state)) || isDrawing;
    }

    private bool isCliffJumping() {
      return cliffJumpRemaining > 0f;
    }

    public void damage(DamageInfo damageInfo) {
      if (isDead) {
        return;
      }
      if (Time.time - lastGetDamagedTime < immutableTime) {
        return;
      }

      if ((isDashing() || isDashKnockingBack()) && !damageInfo.damageDashStatus) {
        return;
      }

      lastGetDamagedTime = Time.time;
      StartCoroutine(Bullets.Utils.ApplyVelocityRepel(damageInfo, rb2d, dizzyTime));
      currentHealth -= damageInfo.damage;
      if (currentHealth <= 0) {
        isDead = true;
      }

      // Vibration
      controllerVibrationManager.RegisterConfig(takeDamageVibrationConfig, true);

      // Dizzy
      cinemachineImpulseSource.GenerateImpulse();
      StartCoroutine(dizzy(damageInfo.setPositionToLastCheckpoint));
      // Effect
      damageEffectParticleSystem.Play(true);
      if (currentHealth <= 0) {
        // Die
        SceneMaster.getInstance().LoadLatest();
        return;
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

    public bool isDashKnockingBack() {
      return dashknowbackTimeslowRemaining > 0f;
    }

    public bool canDash(bool isCliffSliding) {
      return supportDash && remainingDash > 0 && dashCooldownRemaining <= 0.0f && !isCliffSliding;
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

    public void generateJumpUpDust() {
      jumpUpDustParticle?.Play();
    }

    public bool isNearestRelayPoint(RelayPoint relayPoint) {
      return relayPoint == nearestRelayPoint;
    }

    public Snapshot generateSnapshot() {
      Snapshot snapshot = new Snapshot();
      snapshot.currentHp = currentHealth;
      snapshot.maxHp = maxHealth;
      snapshot.canRelayJump = supportRelay;
      snapshot.canStrongShoot = canStrongShoot;
      snapshot.canDash = supportDash;
      snapshot.canCliffJump = canCliffJump;
      snapshot.canTimeslow = supportTimeslow;
      return snapshot;
    }

    public void syncToSnapshot(Snapshot snapshot) {
      currentHealth = snapshot.currentHp;
      maxHealth = snapshot.maxHp;
      supportRelay = snapshot.canRelayJump;
      canStrongShoot = snapshot.canStrongShoot;
      supportDash = snapshot.canDash;
      canCliffJump = snapshot.canCliffJump;
      supportTimeslow = snapshot.canTimeslow;
    }

    private void setTimeScale(TimeScaleConfig config, bool isAnimatorUnscaled = false) {
      timeScaleController.RegisterConfig(config);
      Time.fixedDeltaTime = config.timeScale * DEFAULT_PHYSICS_TIMESTAMP;
      animator.updateMode = isAnimatorUnscaled ? AnimatorUpdateMode.UnscaledTime : AnimatorUpdateMode.Normal;
    }

    private bool canRelay() {
      return !isDizzy
        && !groundSensor.isStay()
        && supportRelay
        && ((nearestRelayPoint != null && getRelayPointDistanceSqrt(nearestRelayPoint) < relayEffectDistance * relayEffectDistance)
          || isRelaySensorTriggered());
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
      foreach (GameObject gameObject in gameObjects) {
        if (!gameObject.CompareTag(Common.Tags.ONESIDE)) {
          return false;
        }
      }
      return true;
    }

    private void exitDash() {
      dashRemainingTime = 0f;
      rb2d.gravityScale = gravityScale;

      if (dashParticle != null) {
        ParticleSystem.EmissionModule emission = dashParticle.emission;
        emission.enabled = false;
      }

      if (dashKnockbackContactDamage != null) {
        dashKnockbackContactDamage.enabled = false;
      }
    }

    // Invoked when dash hit opponent.
    private void onDashKnockbackEvent() {
    }

    private void onDashKnockbackFeedbackEvent(DamageInfo.DamageFeedback damageFeedback) {
      if (damageFeedback.damageSuccess) {
        dashKnockbackSuccessSound?.Play(damageAudioSource);
        if (dashKnockbackParticle != null) {
          ParticleSystem.EmissionModule emission = dashKnockbackParticle.emission;
          dashKnockbackParticle.Play(true);
        }
      }
      // else what?

      // No matter what, exit dash status
      cinemachineImpulseSource.GenerateImpulse();
      // delay exiting dash animiation to have slow motion on dash action
      exitDash();

      if (damageFeedback.damageSuccess) {
        // set DashKnockback status
        rb2d.gravityScale = 0f;
        rb2d.velocity = new Vector2(-Mathf.Sign(getOrientation()) * dashKnowbackRepelSpeed, rb2d.velocity.y);
        dashknowbackTimeslowRemaining = timing.DashKnockFreezeTime;
      }
    }

    private void exitDashKnockback() {
      rb2d.gravityScale = gravityScale;
    }

    private IEnumerator shoot() {
      Debug.Assert(arrowPrefab != null, "Arrow prefab is not set");
      Debug.Assert(shootPoint != null, "Shoot point is not set");

      var arrowSpreadAngel = Mathf.Clamp01(currentBowHeat / maxBowHeat) * maxArrowDirectionSpreadAngle;
      var shootDirection = shootPoint.rotation.eulerAngles;
      shootDirection.z += Random.Range(-arrowSpreadAngel, arrowSpreadAngel);
      float drawingRatio =
        Mathf.Clamp(currentDrawingTime, 0.0f, maxDrawingTime) / maxDrawingTime;
      bool isStrongArrow = canStrongShoot && (drawingRatio > strongArrowDrawingRatio);

      GameObject arrow = Instantiate(
        isStrongArrow ? (isRopeArrow ? ropeArrowPrefab : strongArrowPrefab) : arrowPrefab,
        shootPoint.position + Vector3.forward * 0.1f, Quaternion.Euler(shootDirection));

      // Set arrow 
      TrailRenderer trailRenderer = arrow.GetComponentInChildren<TrailRenderer>();
      trailRenderer.time = Mathf.Lerp(minTrailTime, maxTrailTime, getDrawIntensity());
      ArrowCarrier arrowCarrier = arrow.GetComponent<ArrowCarrier>();

      if (isRopeArrow && isStrongArrow) {
        Bullets.Arrow.Rope rope = arrow.GetComponentInChildren<Bullets.Arrow.Rope>();
        rope.onShoot(arrowRopeAttachPoint, this.gameObject, rope.gameObject.transform, null);
      }

      arrowCarrier.arrowHitMoveableObjectForce = isStrongArrow ? 1000f : 200f;
      arrowCarrier.SetIsShellBreaking(isStrongArrow);
      arrowCarrier.repelIntensive = isStrongArrow ? maxRepelForce : quickRepelForce;

      if (fireArrow && isStrongArrow) {
        arrow.GetComponent<SmallCombustable>()?.ignite();
      }

      float arrowSpeed = isStrongArrow ? strongArrowSpeed : maxArrowSpeed;

      if (enableAutoAim && autoAimGo != null) {
        arrowCarrier.SetAutoAim(autoAimGo);
      }
      arrowCarrier.fire(
        (getOrientation() > 0f ? arrow.transform.right.normalized : -arrow.transform.right.normalized) * arrowSpeed,
        isStrongArrow ? maxArrowLifetime : quickArrowLifetime,
        gameObject.tag,
        weaponPartyConfig);
      isShooting = true;

      // Gamepad Vibration
      //StartCoroutine(ControllerUtils.Vibrate(normalShootVibrateConfig));
      controllerVibrationManager?.RegisterConfig(normalShootVibrateConfig);
      // Player shake
      // Cannot use DOTween because it totally take over the position during shake.
      transform.position += -arrow.transform.right * normalShootShakeConfig.strength;

      // Bow heat
      if (currentBowHeat < maxBowHeat) {
        currentBowHeat += bowHeatIncreasePerShoot;
      }

      // Particle
      shootEffectParticle?.Play(true);

      // Sound effect
      if (shootAudioSource != null) {
        if (isStrongArrow) {
          strongShotSound?.Play(shootAudioSource);
        } else {
          quickShotSound?.Play(shootAudioSource);
        }
      }

      shootingCooldownRemainSeconds = shootingCooldownSeconds;
      rapidShootRemain--;
      yield return null;
      isShooting = false;
    }

    private IEnumerator jumpDown(HashSet<GameObject> onesideGos) {
      List<GameObject> disabledColliders = new List<GameObject>(onesideGos);
      foreach (GameObject gameObject in disabledColliders) {
        Collider2D collider = gameObject.GetComponent<Collider2D>();
        collider.enabled = false;
      }
      yield return new WaitForSeconds(1.0f);
      disabledColliders.ForEach(gameObject => {
        Collider2D collider = gameObject.GetComponent<Collider2D>();
        collider.enabled = true;
      });
    }

    private IEnumerator dizzy(bool setToCheckpointAfterDizzy = false) {
      isDizzy = true;
      yield return new WaitForSeconds(dizzyTime);
      if (currentHealth > 0) {
        isDizzy = false;
        if (setToCheckpointAfterDizzy) {
          Misc.SceneInitializer sceneInitializer = FindObjectOfType<Misc.SceneInitializer>();
          Debug.Assert(sceneInitializer != null, "The scene doesn't have ScenenInitializer.");
          sceneInitializer.loadCheckpoint();
        }
      }
    }

    public void playQuickDrawParticle() {
      quickDrawParticle?.Play();
    }

    public void AssignDustTexture(DustTexture dustTexture) {
      dustTexture.ApplyTexture(dustParticleSystem);
      if (smashParticleSystem != null) {
        foreach (var particleSystem in smashParticleSystem.gameObject.GetComponentsInChildren<ParticleSystem>()) {
          dustTexture.ApplyTexture(particleSystem);
        }
      }
    }

    private void updateFootstepSound() {
      bool isRunning = groundSensor.isStay() && Mathf.Abs(rb2d.velocity.x) > 0.1f;
      if (isRunning && footstepSound != null) {
        footstepSound.PlayIfNotPlaying(audioSource);
      } else {
        audioSource.Stop();
      }
    }

    public void PlayJumpSound() {
      jumpSound?.Play(jumpAudioSource);
    }

    public void PlayDamageSound() {
      damageSound?.Play(damageAudioSource);
    }

    private bool isRelaySensorTriggered() {
      return replyJumpSensor.isStay();
    }

    private void debugDrawAimDirection() {
      Debug.DrawLine(shootPoint.transform.position, shootPoint.transform.position + aimDirection * 10f, Color.red);
    }

    private Vector3 aimDirection => (getOrientation() > 0f ? 1f : -1f) * shootPoint.transform.right;

    private Vector2 autoAim(Vector2 desireDirection) {
      // TODO: Experimental
      if (!enableAutoAim) {
        return desireDirection;
      }
      GameObject aimGO = null;
      float aimAngle = float.MaxValue;


      foreach (Collider2D collider2d in
        Physics2D.OverlapCircleAll(upperBodyGo.transform.position, autoAimDetectRadius)) {

        if (collider2d.gameObject.CompareTag(Tags.PLAYER)
          || (collider2d.gameObject.transform.parent != null && collider2d.gameObject.transform.parent.CompareTag(Tags.PLAYER))) {
          continue;
        }


        // Ignore if no rigidbody2d, to skip trigger
        Rigidbody2D colliderRb2d = collider2d.gameObject.GetComponent<Rigidbody2D>();
        if (colliderRb2d == null) {
          continue;
        }

        // Try to find AutoAimTarget component
        AutoAimTarget[] autoAimTargets = collider2d.gameObject.GetComponentsInChildren<AutoAimTarget>();
        List<GameObject> targetGos = new List<GameObject>();
        if (autoAimTargets != null) {
          foreach (var aimTarget in autoAimTargets) {
            targetGos.Add(aimTarget.gameObject);
          }
        } else {
          if (collider2d.gameObject.layer != Layers.LayerCharacter) {
            continue;
          }
          targetGos.Add(collider2d.gameObject);
        }

        foreach (var targetGo in targetGos) {
          Vector3 delta = targetGo.transform.position - upperBodyGo.transform.position;
          float curAngle = Vector2.Angle(desireDirection, delta);
          float angleToOrientation = Vector2.Angle(new Vector2(getOrientation(), 0f), delta);
          if (curAngle > autoAimCoverAngle || angleToOrientation > maxDirectionAngle) {
            continue;
          }
          if (curAngle < aimAngle) {
            aimGO = targetGo;
            aimAngle = curAngle;
          }
        }
      }
      autoAimGo = aimGO;
      if (aimGO != null) {
        return aimGO.transform.position - upperBodyGo.transform.position;
      }
      return desireDirection;
    }

    private void UpdateAutoAimUi() {
      if (autoAimUiPrefab == null) {
        return;
      }

      // Create auto aim Go if not created.
      if (autoAimUiGo == null) {
        autoAimUiGo = Instantiate(autoAimUiPrefab);
      }

      if (enableAutoAim && autoAimGo != null && input.timeSlow()) {
        autoAimUiGo.transform.position =
          new Vector3(autoAimGo.transform.position.x, autoAimGo.transform.position.y, AxisZ.AUTO_AIM_UI);
        autoAimUiGo.SetActive(true);
      } else {
        autoAimUiGo.SetActive(false);
      }
    }

    public ArrowResult getArrowResult(ArrowCarrier arrowCarrier) {
      return ArrowResult.HIT;
    }

    public void RecoverHealth(int hp) {
      int targetHp = currentHealth + hp;
      currentHealth = targetHp > maxHealth ? maxHealth : targetHp;
    }
  }
}

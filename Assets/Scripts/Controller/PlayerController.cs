using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using Catsland.Scripts.Bullets;
using Catsland.Scripts.Common;
using Catsland.Scripts.Misc;

namespace Catsland.Scripts.Controller {

  [RequireComponent(typeof(IInput))]
  [RequireComponent(typeof(Animator))]
  public class PlayerController: MonoBehaviour {

    // Locomoation
    public float maxRunningSpeed = 1.0f;
    public float maxCrouchSpeed = 0.5f;
    public float acceleration = 1.0f;
    public float jumpForce = 5.0f;
    public float cliffJumpForce = 5.0f;
    public float maxFallingSpeed = 5.0f;
    public float cliffSlidingSpeed = 1.0f;
    public float dashSpeed = 3.0f;
    public float dashTime = 0.6f;
    public float dashCooldown = 1.0f;
    public Transform relayDeterminePoint;
    public float relayHintDistance = 3.0f;
    public float relayEffectDistance = 1.0f;
    public bool supportRelay = true;

    public float maxSenseAdd = 0.5f;
    public float senseIncreaseSpeed = 0.2f;
    public float currentSense = 0.0f;

    private bool isCliffSliding;
    private float dashRemainingTime = 0.0f;
    private float gravityScale = 1.0f;
    private int remainingDash = 1;
    private float dashCooldownRemaining = 0.0f;
    private bool isMeditation = false;

    // Attack
    public float maxArrowSpeed = 15.0f;
    public float minArrowSpeed = 5.0f;
    public float maxArrowLifetime = 3.0f;
    public float maxRepelForce = 50f;
    public float maxDrawingTime = 1.0f;
    private bool isDrawing = false;
    private float shootingCd = 0.5f;
    private bool isShooting = false;
    private float currentDrawingTime = 0.0f;

    private HashSet<RelayPoint> activeRelayPoints = new HashSet<RelayPoint>();

    // Health
    public int maxHealth = 3;
    public int currentHealth;
    public float dizzyTime = 1.0f;
    public float immutableTime = 0.5f;
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
    private ISensor groundSensor;
    private ISensor headSensor;
    private ISensor backSensor;
    private ISensor frontSensor;
    private IInput input;
    private Rigidbody2D rb2d;
    private Animator animator;
    private BoxCollider2D headCollider;
    private SpriteRenderer spriteRenderer;

    // Animation
    private const string H_SPEED = "HSpeed";
    private const string V_SPEED = "VSpeed";
    private const string GROUNDED = "Grounded";
    private const string DRAWING = "Drawing";
    private const string DIZZY = "Dizzy";
    private const string CROUCH = "Crouch";
    private const string CLIFF_SLIDING = "CliffSliding";
    private const string DASHING = "Dashing";

    public void Awake() {
      input = GetComponent<IInput>();
      rb2d = GetComponent<Rigidbody2D>();
      groundSensor = groundSensorGO.GetComponent<ISensor>();
      headSensor = headSenserGo.GetComponent<ISensor>();
      frontSensor = frontSensorGo.GetComponent<ISensor>();
      backSensor = backSensorGo.GetComponent<ISensor>();
      animator = GetComponent<Animator>();
      headCollider = GetComponent<BoxCollider2D>();
      spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Start() {
      currentHealth = maxHealth;
    }

    public void Update() {
      float desiredSpeed = input.getHorizontal();
      float currentVerticleVolocity = rb2d.velocity.y;

      // Cooldown
      if(dashCooldownRemaining > 0.0f) {
        dashCooldownRemaining -= Time.deltaTime;
      }
      if(!input.meditation()) {
        currentSense = Mathf.Max(currentSense - senseIncreaseSpeed * Time.deltaTime, 0.0f);
      }

      // Draw and shoot 
      bool currentIsDrawing =
        input.attack() && !isShooting && !isDizzy && !isDashing() && !input.meditation();
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
        if(input.getVertical() < -0.1f || headSensor.isStay()) {
          // jump down
          if(input.jump() && isAllOneSide(groundSensor.getTriggerGos())) {
            StartCoroutine(jumpDown(groundSensor.getTriggerGos()));
          } else if(isDrawing) {
            // crouch drawing

          } else {
            isCrouching = true;
          }
        } else if(input.jump()) {
          // jump up
          rb2d.velocity = new Vector2(rb2d.velocity.x, 0.0f);
          rb2d.AddForce(new Vector2(0.0f, jumpForce));
        }
      }

      // Relay jump
      if(!isDizzy
        && !groundSensor.isStay()
        && activeRelayPoints.Count > 0
        && input.jump()) {
        rb2d.velocity = Vector2.zero;
        rb2d.AddForce(new Vector2(0.0f, jumpForce));
      }

      // Cliff jump
      float topFallingSpeed = maxFallingSpeed;
      isCliffSliding = false;
      if(!groundSensor.isStay() && !isDizzy) {
        bool desiredFacingOrientation = getOrientation() * desiredSpeed > 0.0f;
        if(frontSensor.isStay() && desiredFacingOrientation) {
          remainingDash = 1;
          if(input.jump()) {
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(-Mathf.Sign(desiredSpeed) * cliffJumpForce, cliffJumpForce));
          } else {
            topFallingSpeed = cliffSlidingSpeed;
            isCliffSliding = true;
          }
        } else if(backSensor.isStay() && desiredFacingOrientation && input.jump()) {
          remainingDash = 1;
          rb2d.velocity = Vector2.zero;
          rb2d.AddForce(new Vector2(Mathf.Sign(desiredSpeed) * cliffJumpForce, cliffJumpForce));
        }
      }

      // Dash
      if(!isDizzy && !input.meditation()) {
        if(isDashing()) {
          dashRemainingTime -= Time.deltaTime;
          if(dashRemainingTime < 0.0f) {
            // exit dash
            rb2d.gravityScale = gravityScale;
          }
        } else if(input.dash() && canDash()) {
          // enter dash
          rb2d.velocity = new Vector2(Mathf.Sign(getOrientation()) * dashSpeed, 0.0f);
          gravityScale = rb2d.gravityScale;
          rb2d.gravityScale = 0.0f;
          dashRemainingTime = dashTime;
          remainingDash = 0;
          dashCooldownRemaining = dashCooldown;
        }
      }

      // meditation
      isMeditation = false;
      if(!isDizzy && groundSensor.isStay() && input.meditation()) {
        currentSense = Mathf.Min(maxSenseAdd, currentSense + senseIncreaseSpeed * Time.deltaTime);
        isMeditation = true;
      }

      // horizontal movement
      gameObject.transform.parent =
        groundSensor.isStay() ? Utils.getAnyFrom(groundSensor.getTriggerGos()).transform : null;
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

      // Update facing
      if(Mathf.Abs(desiredSpeed) > Mathf.Epsilon && !isDizzy && !input.meditation()) {
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

      // Update head collider
      if(headCollider != null) {
        headCollider.enabled = !isCrouching;
      }

      // Update animation
      animator.SetBool(GROUNDED, groundSensor.isStay());
      animator.SetFloat(H_SPEED, Mathf.Abs(rb2d.velocity.x));
      animator.SetFloat(V_SPEED, rb2d.velocity.y);
      animator.SetBool(DRAWING, isDrawing);
      animator.SetBool(DIZZY, isDizzy);
      animator.SetBool(CROUCH, isCrouching);
      animator.SetBool(CLIFF_SLIDING, isCliffSliding);
      animator.SetBool(DASHING, isDashing());
    }

    public void damage(DamageInfo damageInfo) {
      if(Time.time - lastGetDamagedTime < immutableTime) {
        return;
      }

      lastGetDamagedTime = Time.time;
      rb2d.velocity = Vector2.zero;
      rb2d.AddForce(damageInfo.repelDirection * damageInfo.repelIntense);
      currentHealth -= damageInfo.damage;
      if(currentHealth <= 0) {
        // Die
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      } else {
        // Dizzy
        StartCoroutine(dizzy());
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

    public bool canDash() {
      return remainingDash > 0 && dashCooldownRemaining <= 0.0f;
    }

    public void registerRelayPoint(RelayPoint relayPoint) {
      activeRelayPoints.Add(relayPoint);
    }

    public void unregisterRelayPoint(RelayPoint relayPoint) {
      activeRelayPoints.Remove(relayPoint);
    }

    private bool isAllOneSide(HashSet<GameObject> gameObjects) {
      foreach(GameObject gameObject in gameObjects) {
        if(!gameObject.CompareTag(Tags.ONESIDE)) {
          return false;
        }
      }
      return true;
    }

    private IEnumerator shoot() {
      Debug.Assert(arrowPrefab != null, "Arrow prefab is not set");
      Debug.Assert(shootPoint != null, "Shoot point is not set");

      GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
      // Set arrow ordering layer
      SpriteRenderer renderer = arrow.GetComponent<SpriteRenderer>();
      if(renderer != null) {
        renderer.sortingOrder = spriteRenderer.sortingOrder + 1;
      }
      ParticleSystem particleSystem = arrow.GetComponentInChildren<ParticleSystem>();
      if(particleSystem != null) {
        particleSystem.gameObject.GetComponent<ParticleSystemRenderer>().sortingOrder = spriteRenderer.sortingOrder;
      }
      TrailRenderer trailRenderer = arrow.GetComponentInChildren<TrailRenderer>();
      if(trailRenderer != null) {
        trailRenderer.sortingOrder = spriteRenderer.sortingOrder;
      }
      ArrowCarrier arrowCarrier = arrow.GetComponent<ArrowCarrier>();
      float drawingRatio =
        Mathf.Clamp(currentDrawingTime, 0.0f, maxDrawingTime) / maxDrawingTime;
      arrowCarrier.repelIntensive = drawingRatio * maxRepelForce;
      float absoluteArrowSpeed = Mathf.Lerp(minArrowSpeed, maxArrowSpeed, drawingRatio);
      StartCoroutine(arrowCarrier.fire(
        new Vector2(transform.lossyScale.x > 0.0f ? absoluteArrowSpeed : -absoluteArrowSpeed, 0.0f),
        maxArrowLifetime,
        gameObject.tag));
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
    }
  }
}

using UnityEngine;
using System.Collections.Generic;

namespace Catslandx {

  public class NinjiaController: AbstractCharacterController {
    
    public float groundedRadius;
    public LayerMask whatIsGround;
    public bool supportRelay = true;
    public float maxDashSpeed = 4.0f;
    public float maxGroundSpeed = 3.0f;
    public float dashSlowdown = 0.05f;
    public float minDashSpeed = 1.0f;
    public float jumpForce = 1.0f;
    public float maxCrouchSpeed = 1.0f;
    public float dizzyDuration = 2.0f;
    public float airAdjustmentScale = 0.5f;
    public float jumpHorizontalSpeedDump = 0.5f;
    public float relayTimeScale = 0.3f;
    public Transform standHeadCheckPoint;
    public Transform groundCheckPoint;
    public ParticleSystem particalSystem;
    public GameObject deathLightPrefab;

    private bool isGrounded;
    private bool isDash;
    private bool isCrouch;
    private bool isDead;
    private RelayPoint relayPoint;
    private bool isFaceRight = true;
    private float dizzyTimeLeft = 0.0f;
    private GameObject groundObject;

    private Rigidbody2D rigidbody;
    private Animator animator;
    private BoxCollider2D boxCollider2D;

    /// input
    private ICharacterInput characterInput;

    /// sensors
    private Dictionary<SensorEnum, ISensor> sensors;

    /// <summary>
    /// Abilities
    /// </summary>
    // deprecated
    private CharacterVulnerable characterVulnerable;
    private HealthAbility healthAbility;

    /// <summary>
    /// inside controller
    /// </summary>
    private IState currentState;
    

    public float runSoundRippleCycleSecond = 0.7f;
    public float runVolume = 0.8f;
    public float landVolume = 1.0f;
    private float currentSoundRippleCycleSecond = 0.0f;
    private IRespawn respawn;

    private void Awake() {
      rigidbody = GetComponent<Rigidbody2D>();
      animator = GetComponent<Animator>();
      boxCollider2D = GetComponent<BoxCollider2D>();

      characterInput = GetComponent<ICharacterInput>();

      // deprecated
      characterVulnerable = GetComponent<CharacterVulnerable>();
      healthAbility = GetComponent<HealthAbility>();

      // TODO: whether it is ability?
      respawn = GetComponent<IRespawn>();
    }

    protected void updateSensor() {
      // TODO: whether to extract it as ISensor
      bool tempIsGrounded = false;
      Collider2D[] colliders = Physics2D.OverlapCircleAll(
        groundCheckPoint.position, groundedRadius, whatIsGround);
      for(int i = 0; i < colliders.Length; ++i) {
        if(colliders[i].gameObject != gameObject) {
          tempIsGrounded = true;
          groundObject = colliders[i].gameObject;
          break;
        }
      }
      // TODO: should move out of this function
      if(tempIsGrounded && !isGrounded) {
        land();
      } else if(!tempIsGrounded && isGrounded) {
        takeOff();
        groundObject = null;
      }
      isGrounded = tempIsGrounded;
      if(dizzyTimeLeft > 0.0f) {
        dizzyTimeLeft -= Time.fixedDeltaTime;
      }
      updateLoopSound();
      updateParticle();

    }

    private void FixedUpdate() {
      updateSensor();
    }

    // TODO: should not be in this class 
    private void updateParticle() {
      if(particalSystem != null) {
        if(isGrounded && groundObject != null) {
          GroundMaterial groundMaterial = groundObject.GetComponent<GroundMaterial>();
          if(groundMaterial != null) {
            particalSystem.enableEmission = true;
            particalSystem.startColor = groundMaterial.dustColor;
            ParticleSystem.EmissionModule emission = particalSystem.emission;
            emission.rate = new ParticleSystem.MinMaxCurve(groundMaterial.dustRate);
            emission.SetBursts(new ParticleSystem.Burst[] {
              new ParticleSystem.Burst(0.0f, groundMaterial.minDustBurst, groundMaterial.maxDustBurst) });
            return;
          }
        }
        particalSystem.enableEmission = false;
      }
    }

    public void getHurt(int hurtPoint) {
      if(!isDash) {
        dizzyTimeLeft = dizzyDuration;
      }
    }

    public bool isDizzy() {
      return dizzyTimeLeft > 0.0f;
    }

    public bool getIsFaceRight() {
      return isFaceRight;
    }

    public void update() {
      updateMovement(Time.deltaTime);
    }

    protected void updateMovement(float deltaTime) {
      IState nextState = currentState.update(characterInput, deltaTime);
      if(nextState != currentState) {
        currentState.onExit(nextState);
        nextState.onEnter(currentState);
        currentState = nextState;
      }
    }

    public void transitToState(IState state) {
      // TODO
    }

    public void move(Vector2 move, bool jump, bool dash, bool crouch) {
      if(!isDizzy()) {
        if(isDash) {
          handleDashMovement(move, jump, dash);
        } else {
          if(isGrounded) {
            handleGroundedMovement(move, crouch, jump);
          } else {
            handleAirboneMovement(move, jump, dash);
          }
        }
        updateOrientation(move);
      }

    }

    private void handleDashMovement(Vector2 move, bool jump, bool dash) {
      if(supportRelay && relayPoint != null && (jump || dash)) {
        handleAirboneMovement(move, jump, dash);
      } else {
        rigidbody.velocity = Vector2.Lerp(rigidbody.velocity, Vector2.zero, dashSlowdown);
        if(rigidbody.velocity.sqrMagnitude < minDashSpeed * minDashSpeed) {
          exitDash();
        }
      }
    }

    private void enterDash() {
      isDash = true;
      rigidbody.gravityScale = 0.0f;
      if(characterVulnerable != null) {
        characterVulnerable.setCanGetHurt(false);
      }
    }

    private void exitDash() {
      isDash = false;
      rigidbody.gravityScale = 1.0f;
      if(characterVulnerable != null) {
        characterVulnerable.setCanGetHurt(true);
      }
    }

    private void handleGroundedMovement(Vector2 move, bool crouch, bool jump) {
      if(isHeadOnCeiling()) {
        crouch = true;
      }
      isCrouch = crouch;
      float maxSpeed = crouch ? maxCrouchSpeed : maxGroundSpeed;
      float horizontalSpeed = move.x * maxSpeed;
      float verticalTargetSpeed = rigidbody.velocity.y;
      if(jump && !crouch) {
        verticalTargetSpeed = jumpForce;
      }
      rigidbody.velocity = new Vector2(horizontalSpeed, verticalTargetSpeed);
    }

    private void handleAirboneMovement(Vector2 move, bool jump, bool dash) {
      isCrouch = false;
      if(supportRelay && relayPoint != null) {
        if(jump) {
          if(isDash) {
            // exit dash
            exitDash();
          }
          relayPoint.jumpOnRelay(gameObject);
          rigidbody.velocity = new Vector2(move.x * maxGroundSpeed, jumpForce);
          return;
        } else if(dash) {
          // enter dash
          enterDash();
          // decide the dash direction
          if(move.y * move.y > move.x * move.x) {
            // vertical dash
            relayPoint.dashOnRelay(gameObject);
            rigidbody.velocity = new Vector2(0.0f, Mathf.Sign(move.y) * maxDashSpeed);
            return;

          } else {
            float dashHorizontalSpeed = maxDashSpeed;
            if(move.x > 0.0f) {
              dashHorizontalSpeed = maxDashSpeed;
            } else if(move.x < 0.0f) {
              dashHorizontalSpeed = -maxDashSpeed;
            } else {
              dashHorizontalSpeed = isFaceRight ? maxDashSpeed : -maxDashSpeed;
            }
            relayPoint.dashOnRelay(gameObject);
            rigidbody.velocity = new Vector2(dashHorizontalSpeed, 0.0f);
            return;
          }
        }
      }
      // normal air adjustment
      //rigidbody.position += Vector2.right * move * airAdjustmentScale * Time.deltaTime;
      rigidbody.velocity = new Vector2(move.x * maxGroundSpeed, rigidbody.velocity.y);
    }

    private void updateOrientation(Vector2 move) {
      if(isDash) {
        isFaceRight = rigidbody.velocity.x > 0.0f;
      } else {
        if(move.x > 0.01f) {
          isFaceRight = true;
        } else if(move.x < -0.01f) {
          isFaceRight = false;
        }
      }
    }

    private void updateAnimation() {
      animator.SetFloat("horizontalAbsSpeed", Mathf.Abs(rigidbody.velocity.x));
      animator.SetBool("isGrounded", isGrounded);
      animator.SetBool("isDashing", isDash);
      animator.SetBool("isCrouching", isCrouch);
      animator.SetFloat("verticalSpeed", rigidbody.velocity.y);
      if((isFaceRight && transform.localScale.x < 0.0f)
        || (!isFaceRight && transform.localScale.x > 0.0f)) {
        transform.localScale = new Vector3(
          -transform.localScale.x, transform.localScale.y, transform.localScale.z);
      }
    }

    public void die() {
      if(!isDead) {
        if(deathLightPrefab != null) {
          GameObject deathLight = Instantiate(deathLightPrefab);
          deathLight.transform.position = transform.position;
          ParticleSystem particle = deathLight.GetComponent<ParticleSystem>();
          if(particle != null) {
            particle.Play();
          }
        }
        if(respawn != null) {
          respawn.doRespawn();
        }
        isDead = true;
      }
    }

    private bool isHeadOnCeiling() {
      bool hitCeiling = false;

      Collider2D[] colliders = Physics2D.OverlapCircleAll(
        standHeadCheckPoint.position, groundedRadius, whatIsGround);

      for(int i = 0; i < colliders.Length; ++i) {
        if(colliders[i].gameObject != gameObject) {
          hitCeiling = true;
        }
      }
      return hitCeiling;
    }

    // Use this for initialization
    void Start() {
      reset();
    }

    // Update is called once per frame
    void Update() {

      float deltaTime = Time.deltaTime;

      updateMovement();

      updateAnimation();
    }

    public bool isSupportRelay() {
      return supportRelay;
    }

    public bool setRelayPoint(RelayPoint relayPoint) {
      this.relayPoint = relayPoint;
      Time.timeScale = relayTimeScale;
      return true;
    }

    public bool cancelRelayPoint(RelayPoint relayPoint) {
      if(this.relayPoint == relayPoint) {
        this.relayPoint = null;
        Time.timeScale = 1.0f;
        return true;
      } else {
        return false;
      }
    }

    private void land() {
      // make a sound
      SoundRipple.createRipple(
        landVolume,
        boxCollider2D.transform.position + new Vector3(0.0f, -boxCollider2D.size.y * transform.localScale.y / 2.0f, 0.0f),
        gameObject);
      if(particalSystem != null) {
        particalSystem.Play();
      }
      currentSoundRippleCycleSecond += runSoundRippleCycleSecond;
    }

    private void takeOff() {

    }

    private void updateLoopSound() {
      if(isGrounded) {
        if(!isCrouch && Mathf.Abs(rigidbody.velocity.x) > 0.1f) {
          currentSoundRippleCycleSecond -= Time.deltaTime;
          if(currentSoundRippleCycleSecond < 0.0f) {
            SoundRipple.createRipple(
              runVolume,
              boxCollider2D.transform.position + new Vector3(0.0f, -boxCollider2D.size.y * transform.localScale.y / 2.0f, 0.0f),
              gameObject);
            if(particalSystem != null) {
              particalSystem.Play();
            }
            currentSoundRippleCycleSecond += runSoundRippleCycleSecond;
          }
        }
      }
    }

    public void reset() {
      isDead = false;
    }
  }
}

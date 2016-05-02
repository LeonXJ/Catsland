using UnityEngine;
using System.Collections;
using System;

namespace Catslandx {
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(Animator))]
  public class CharacterController2DNinja : MonoBehaviour, IRelayPointCatcher, ICharacterController2D {

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
    public Transform standHeadCheckPoint;
    public Transform groundCheckPoint;

    private bool isGrounded;
    private bool isDash;
    private bool isCrouch;
    private RelayPoint relayPoint;
    private bool isFaceRight = true;
    private float dizzyTimeLeft = 0.0f;

    private Rigidbody2D rigidbody;
    private Animator animator;
    private BoxCollider2D boxCollider2D;
    private CharacterVulnerable characterVulnerable;

    public float runSoundRippleCycleSecond = 0.7f;
    public float runVolume = 0.8f;
    public float landVolume = 1.0f;
    private float currentSoundRippleCycleSecond = 0.0f;

    private void Awake() {
      rigidbody = GetComponent<Rigidbody2D>();
      animator = GetComponent<Animator>();
      boxCollider2D = GetComponent<BoxCollider2D>();
      characterVulnerable = GetComponent<CharacterVulnerable>();
    }

    private void FixedUpdate() {
      bool tempIsGrounded = false;
      Collider2D[] colliders = Physics2D.OverlapCircleAll(
        groundCheckPoint.position, groundedRadius, whatIsGround);
      for (int i=0; i<colliders.Length; ++i) {
        if (colliders[i].gameObject != gameObject) {
          tempIsGrounded = true;
          break;
        }
      }
      if (tempIsGrounded && !isGrounded) {
        land();
      } else if (!tempIsGrounded && isGrounded) {
        takeOff();
      }
      isGrounded = tempIsGrounded;



      if (dizzyTimeLeft > 0.0f) {
        dizzyTimeLeft -= Time.fixedDeltaTime;
      }
      updateLoopSound();
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

    public void move(float move, bool jump, bool dash, bool crouch) {
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

    private void handleDashMovement(float move, bool jump, bool dash) {
      if (supportRelay && relayPoint != null && (jump || dash)) {
        handleAirboneMovement(move, jump, dash);
      } else {
        rigidbody.velocity = Vector2.Lerp(rigidbody.velocity, Vector2.zero, dashSlowdown);
        if (Mathf.Abs(rigidbody.velocity.x) < minDashSpeed) {
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
      if (characterVulnerable != null) {
        characterVulnerable.setCanGetHurt(true);
      }
    }

    private void handleGroundedMovement(float move, bool crouch, bool jump) {
      if (isHeadOnCeiling()) {
        crouch = true;
      }
      isCrouch = crouch;
      float maxSpeed = crouch ? maxCrouchSpeed : maxGroundSpeed;
      rigidbody.velocity = new Vector2(move * maxSpeed, rigidbody.velocity.y);
      if (jump && !crouch) {
        rigidbody.velocity = new Vector2(move * maxSpeed * jumpHorizontalSpeedDump, jumpForce);
      }
    }

    private void handleAirboneMovement(float move, bool jump, bool dash) {
      isCrouch = false;
      if (supportRelay && relayPoint != null) {
        if (jump) {
          if (isDash) {
            // exit dash
            exitDash();
          }
          rigidbody.velocity = new Vector2(move * maxGroundSpeed, jumpForce);
          return;
        } else if (dash) {
          // enter dash
          enterDash();
          // decide the dash direction
          float dashHorizontalSpeed = maxDashSpeed;
          if (move > 0.0f) {
            dashHorizontalSpeed = maxDashSpeed;
          } else if (move < 0.0f) {
            dashHorizontalSpeed = -maxDashSpeed;
          } else {
            dashHorizontalSpeed = isFaceRight ? maxDashSpeed : -maxDashSpeed;
          }
          rigidbody.velocity = new Vector2(dashHorizontalSpeed, 0.0f);
          return;
        }
      }
      // normal air adjustment
      rigidbody.position += Vector2.right * move * airAdjustmentScale * Time.deltaTime;
    }

    private void updateOrientation(float move) {
      if (isDash) {
        isFaceRight = rigidbody.velocity.x > 0.0f;
      } else {
        if (move > 0.01f) {
          isFaceRight = true;
        } else if (move < -0.01f) {
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
      if ((isFaceRight && transform.localScale.x < 0.0f)
        || (!isFaceRight && transform.localScale.x > 0.0f)) {
        transform.localScale = new Vector3(
          -transform.localScale.x, transform.localScale.y, transform.localScale.z);
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
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
      updateAnimation();
    }

    public bool isSupportRelay() {
      return supportRelay;
    }

    public bool setRelayPoint(RelayPoint relayPoint) {
      this.relayPoint = relayPoint;
      return true;
    }

    public bool cancelRelayPoint(RelayPoint relayPoint) {
      if (this.relayPoint == relayPoint) {
        this.relayPoint = null;
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
      currentSoundRippleCycleSecond += runSoundRippleCycleSecond;
    }

    private void takeOff() {

    }

    private void updateLoopSound() {
      if (isGrounded) {
        if(!isCrouch && Mathf.Abs(rigidbody.velocity.x) > 0.1f) {
          currentSoundRippleCycleSecond -= Time.deltaTime;
          if (currentSoundRippleCycleSecond < 0.0f) {
            SoundRipple.createRipple(
              runVolume,
              boxCollider2D.transform.position + new Vector3(0.0f, -boxCollider2D.size.y * transform.localScale.y / 2.0f, 0.0f),
              gameObject);
            currentSoundRippleCycleSecond += runSoundRippleCycleSecond;
          }
        }
      }
    }
  }
}

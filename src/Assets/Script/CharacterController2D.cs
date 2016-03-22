using UnityEngine;
using System.Collections;
using System;

namespace Catslandx {
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(Animator))]
  public class CharacterController2D : MonoBehaviour, IRelayPointCatcher{

    public Transform groundCheckPosition;
    public float groundedRadius;
    public LayerMask whatIsGround;
    public bool supportRelay = true;
    public float maxDashSpeed = 4.0f;
    public float maxGroundSpeed = 3.0f;
    public float dashSlowdown = 0.05f;
    public float minDashSpeed = 1.0f;
    public float jumpForce = 1.0f;

    private bool isGrounded;
    private bool isDash;
    private RelayPoint relayPoint;
    private bool isFaceRight = true;

    private Rigidbody2D rigidbody;
    private Animator animator;

    private void Awake() {
      rigidbody = GetComponent<Rigidbody2D>();
      animator = GetComponent<Animator>();
    }

    private void FixedUpdate() {
      isGrounded = false;
      Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPosition.position, groundedRadius, whatIsGround);
      for (int i=0; i<colliders.Length; ++i) {
        if (colliders[i].gameObject != gameObject) {
          isGrounded = true;
        }
      }
    }

    public void move(float move, bool crouch, bool jump, bool dash) {
      if (isDash) {
        handleDashMovement(move, jump, dash);
      } else {
        if (isGrounded) {
          handleGroundedMovement(move, crouch, jump);
        } else {
          handleAirboneMovement(move, jump, dash);
        }
      }
      updateOrientation(move);
      updateAnimation();
    }

    private void handleDashMovement(float move, bool jump, bool dash) {
      if (supportRelay && relayPoint != null && (jump || dash)) {
        handleAirboneMovement(move, jump, dash);
      } else {
        rigidbody.velocity = Vector2.Lerp(rigidbody.velocity, Vector2.zero, dashSlowdown);
        if (rigidbody.velocity.x < minDashSpeed) {
          rigidbody.gravityScale = 1.0f;
          isDash = false;
        }
      }
    }

    private void handleGroundedMovement(float move, bool crouch, bool jump) {
      // not support crouch so far
      rigidbody.velocity = new Vector2(move * maxGroundSpeed, rigidbody.velocity.y);
      if (jump) {
        rigidbody.AddForce(new Vector2(0.0f, jumpForce));
      }
    }

    private void handleAirboneMovement(float move, bool jump, bool dash) {
      if (supportRelay && relayPoint != null) {
        if (jump) {
          if (isDash) {
            // exit dash
            rigidbody.gravityScale = 1.0f;
            isDash = false;
          }
          rigidbody.velocity = new Vector2(move * maxGroundSpeed, rigidbody.velocity.y);
          rigidbody.AddForce(new Vector2(0.0f, jumpForce));
        } else if (dash) {
          // enter dash
          rigidbody.gravityScale = 0.0f;
          isDash = true;
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
        }
      }
    }

    private void updateOrientation(float move) {
      if (isDash) {
        isFaceRight = rigidbody.velocity.x > 0.0f;
      } else {
        isFaceRight = move > 0.0f;
      }
    }

    private void updateAnimation() {
      animator.SetFloat("speed", Mathf.Abs(rigidbody.velocity.x));
      animator.SetBool("isGrounded", isGrounded);
      animator.SetBool("isDash", isDash);
      //animator.SetBool("faceRight", isFaceRight);
      if (isFaceRight) {
        transform.localScale = Vector3.one;
      } else {
        transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
      }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
  }
}

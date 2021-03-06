﻿using UnityEngine;
using Catslandx.Script.CharacterController.Common;
using Catslandx.Script.Sensor;

namespace Catslandx.Script.CharacterController.Ninja {

  /** Ninja's controller. */
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(Collider2D))]
  public class NinjaController: AbstractCharacterController {

    public static class AnimatorValue {
      public static string horizontalSpeed = "horizontalAbsSpeed";
      public static string isGrounded = "isGrounded";
      public static string isAttacking = "isAttacking";
      public static string meleeLevel = "meleeLevel";
      public static string isHurted = "isHurted";
      public static string isShooting = "isShooting";
    }

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

    private bool isDash;
    private bool isCrouch;
    private bool isDead;
    private RelayPoint relayPoint;
    private bool isFaceRight = true;
    private float dizzyTimeLeft = 0.0f;
    private GameObject groundObject;

    private new Rigidbody2D rigidbody;
    private Animator animator;
    private Collider2D collider2D;

    /// sensors
    public GameObject groundSensorGO;
    public GameObject headSensorGO;
    public GameObject relaySensorGO;
	public GameObject rearSensorGO;
	public GameObject frontSensorGO;

    // Ablilities
    private MovementAbility movementAbility;

    // deprecated
    private CharacterVulnerable characterVulnerable;
    private HealthAbility healthAbility;

    public float runSoundRippleCycleSecond = 0.7f;
    public float runVolume = 0.8f;
    public float landVolume = 1.0f;
    private float currentSoundRippleCycleSecond = 0.0f;
    private IRespawn respawn;

    /** Get required components.
     * 
     * Called by Unity.
     */ 
    private void Awake() {
      rigidbody = GetComponent<Rigidbody2D>();
      animator = GetComponent<Animator>();
      collider2D = GetComponent<Collider2D>();
      movementAbility = GetComponent<MovementAbility>();
    }

    protected override IStatus getInitialStatus() {
      return stateFactory.getState<MovementStatus>();
    }

    protected override void initializeSensor() {
      // Ground sensor.
      if (groundSensorGO != null) {
        IPreCalculateSensor groundSendor = 
          groundSensorGO.GetComponent<PreCalculateSensor>();
        sensors.Add(SensorEnum.ON_GROUND_SENSOR, groundSendor);
      }
      // Head sensor
      if(headSensorGO != null) {
        IPreCalculateSensor headSensor =
          headSensorGO.GetComponent<PreCalculateSensor>();
        sensors.Add(SensorEnum.CEILING_SENSOR, headSensor);
      }
      // Head sensor
      if(relaySensorGO!= null) {
        IPreCalculateSensor relaySensor =
          headSensorGO.GetComponent<PreCalculateSensor>();
        sensors.Add(SensorEnum.RELAY_SENSOR, relaySensor);
      }

	  // Left sensor
	  if(rearSensorGO != null) {
		IPreCalculateSensor rearSensor =
		  rearSensorGO.GetComponent<PreCalculateSensor>();
		sensors.Add(SensorEnum.REAR_SENSOR, rearSensor);
	  }

	  // Right sensor
	  if(frontSensorGO != null) {
		IPreCalculateSensor frontSensor =
		  frontSensorGO.GetComponent<PreCalculateSensor>();
		sensors.Add(SensorEnum.FRONT_SENSOR, frontSensor);
	  }
    }

    protected override void updateAnimation(float deltaTime) {
      animator.SetFloat(AnimatorValue.horizontalSpeed, Mathf.Abs(rigidbody.velocity.x));
      animator.SetBool("isDashing", isDash);
      animator.SetBool("isCrouching", movementAbility.getIsCrouch());
      animator.SetFloat("verticalSpeed", rigidbody.velocity.y);
      animator.SetBool(AnimatorValue.isAttacking, currentStatus.GetType() == typeof(MeleeStatus));
      animator.SetBool(AnimatorValue.isHurted, currentStatus.GetType() == typeof(HurtStatus));
      animator.SetBool(AnimatorValue.isShooting, currentStatus.GetType() == typeof(ShootStatus));

      // Sets isGrounded.
      ISensor groundSensor = sensors[SensorEnum.ON_GROUND_SENSOR];
      if(groundSensor != null) {
        animator.SetBool(AnimatorValue.isGrounded, groundSensor.isInTrigger());
      } else {
        animator.SetBool(AnimatorValue.isGrounded, false);
      }

      // Sets orientation.
      if((getOrientation() == Orientation.Right 
        && transform.localScale.x < 0.0f)
        || (getOrientation() == Orientation.Left 
        && transform.localScale.x > 0.0f)) {
        transform.localScale = new Vector3(
          -transform.localScale.x, transform.localScale.y, transform.localScale.z);
      }

      currentStatus.applyAnimation(animator);
    }
  }
}

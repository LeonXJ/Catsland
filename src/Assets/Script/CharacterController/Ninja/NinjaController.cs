using UnityEngine;
using Catslandx.Script.CharacterController.Common;
using Catslandx.Script.Sensor;

namespace Catslandx.Script.CharacterController.Ninja {

  /** Ninja's controller. */
  [RequireComponent(typeof(Rigidbody2D))]
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(BoxCollider2D))]
  public class NinjaController: AbstractCharacterController {

    private static class AnimatorValue {
      public static string horizontalSpeed = "horizontalAbsSpeed";
      public static string isGrounded = "isGrounded";
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
    private BoxCollider2D boxCollider2D;

    /// sensors
    public GameObject groundSensorGO;
    public GameObject headSensorGO;
    public GameObject relaySensorGO;

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
      boxCollider2D = GetComponent<BoxCollider2D>();
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
    }

    protected override void updateAnimation(float deltaTime) {
      animator.SetFloat(AnimatorValue.horizontalSpeed, Mathf.Abs(rigidbody.velocity.x));
      animator.SetBool("isDashing", isDash);
      animator.SetBool("isCrouching", movementAbility.getIsCrouch());
      animator.SetFloat("verticalSpeed", rigidbody.velocity.y);

      // Sets isGrounded.
      ISensor groundSensor = sensors[SensorEnum.ON_GROUND_SENSOR];
      if(groundSensor != null) {
        //bool onGround = groundSensor.isInTrigger();
        animator.SetBool(AnimatorValue.isGrounded, groundSensor.isInTrigger());
      } else {
        animator.SetBool(AnimatorValue.isGrounded, false);
      }

      // Sets orientation.
      if((movementAbility.getOrientation() == MovementAbility.Orientation.Right 
        && transform.localScale.x < 0.0f)
        || (movementAbility.getOrientation() == MovementAbility.Orientation.Left 
        && transform.localScale.x > 0.0f)) {
        transform.localScale = new Vector3(
          -transform.localScale.x, transform.localScale.y, transform.localScale.z);
      }
    }
  }
}

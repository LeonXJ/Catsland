using System.Collections;
using UnityEngine;

using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.CharacterController {

  [RequireComponent(typeof(IInput))]
  [RequireComponent(typeof(Animator))]
  public class PlayerController :MonoBehaviour {

    // Locomoation
    public float maxHorizontalSpeed = 1.0f;
    public float acceleration = 1.0f;
    public float jumpForce = 5.0f;


    // Attack
    public float arrowSpeed = 5.0f;
    public float arrowLifetime = 3.0f;
    private bool isDrawing = false;
    private float shootingCd = 0.5f;
    private bool isShooting = false;

    // References
    public GameObject groundSensorGO;
    public GameObject arrowPrefab;
    public Transform shootPoint;
    private ISensor groundSensor;
    private IInput input;
    private Rigidbody2D rb2d;
    private Animator animator;

    // Animation
    private const string H_SPEED = "HSpeed";
    private const string V_SPEED = "VSpeed";
    private const string GROUNDED = "Grounded";
    private const string DRAWING = "Drawing";

    public void Awake() {
      input = GetComponent<IInput>();
      rb2d = GetComponent<Rigidbody2D>();
      groundSensor = groundSensorGO.GetComponent<ISensor>();
      animator = GetComponent<Animator>();
    }

    public void Update() {
      float desiredSpeed = input.getHorizontal();
      float currentVerticleVolocity = rb2d.velocity.y;
      bool verticleStable = Mathf.Abs(currentVerticleVolocity) < 0.1f;

      // Draw and shoot 
      bool currentIsDrawing =
        groundSensor.isStay() && verticleStable && input.attack() && !isShooting;
      // Shoot if string is released
      if(isDrawing && !currentIsDrawing) {
        StartCoroutine(shoot());
      }
      isDrawing = currentIsDrawing;
      // Update shooting cd

      // Movement
      if(groundSensor.isStay() && verticleStable) {
        if(input.jump()) {
          rb2d.AddForce(new Vector2(0.0f, jumpForce));
        }
      }
      gameObject.transform.parent =
        groundSensor.isStay() ? groundSensor.getTriggerGO().transform : null;
      if(!isDrawing && Mathf.Abs(desiredSpeed) > Mathf.Epsilon) {
        rb2d.AddForce(new Vector2(acceleration * desiredSpeed, 0.0f));
        rb2d.velocity = new Vector2(
          Mathf.Clamp(rb2d.velocity.x, -maxHorizontalSpeed, maxHorizontalSpeed),
          rb2d.velocity.y);
      } else {
        rb2d.velocity = new Vector2(0.0f, rb2d.velocity.y);
      }

      // Update facing
      if(Mathf.Abs(desiredSpeed) > Mathf.Epsilon) {
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

      // Update animation
      animator.SetBool(GROUNDED, groundSensor.isStay());
      animator.SetFloat(H_SPEED, Mathf.Abs(rb2d.velocity.x));
      animator.SetFloat(V_SPEED, rb2d.velocity.y);
      animator.SetBool(DRAWING, isDrawing);
    }

    public void damage(DamageInfo damageInfo) {
      Debug.Log("DEBUG>>> take damage");
      rb2d.AddForce(damageInfo.repelDirection * damageInfo.repelIntense);
    }

    private IEnumerator shoot() {
      Debug.Assert(arrowPrefab != null, "Arrow prefab is not set");
      Debug.Assert(shootPoint != null, "Shoot point is not set");

      GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
      ArrowCarrier arrowCarrier = arrow.GetComponent<ArrowCarrier>();
      Debug.Log("Lossy Scale X: " + transform.lossyScale.x);
      StartCoroutine(arrowCarrier.fire(
        new Vector2(transform.lossyScale.x > 0.0f ? arrowSpeed : -arrowSpeed, 0.0f),
        arrowLifetime,
        gameObject.tag));
      isShooting = true;
      yield return new WaitForSeconds(shootingCd);
      isShooting = false;
    }
  }
}

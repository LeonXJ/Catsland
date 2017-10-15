using UnityEngine;

namespace Catsland.Scripts.CharacterController {

  [RequireComponent(typeof(IInput))]
  [RequireComponent(typeof(Animator))]
  public class PlayerController :MonoBehaviour {

    // Locomoation
    public float maxHorizontalSpeed = 1.0f;
    public float acceleration = 1.0f;
    public float jumpForce = 5.0f;


    // Attack
    public bool isDrawing = false;

    // References
    public GameObject groundSensorGO;
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


      // Attack
      isDrawing = groundSensor.isStay() && input.attack();

      // Movement
      if(groundSensor.isStay()) {
        if(input.jump()) {
          rb2d.AddForce(new Vector2(0.0f, jumpForce));
        }
      }
      if(!isDrawing && Mathf.Abs(desiredSpeed) > Mathf.Epsilon) {
        rb2d.AddForce(new Vector2(acceleration * desiredSpeed, 0.0f));
        rb2d.velocity = new Vector2(
          Mathf.Clamp(rb2d.velocity.x, -maxHorizontalSpeed, maxHorizontalSpeed),
          rb2d.velocity.y);
      } else {
        rb2d.velocity = new Vector2(0.0f, rb2d.velocity.y);
      }

      // Update facing
      float orientation = getOrientation();
      if(orientation > 0.0f) {
        transform.localScale = new Vector3(
          Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
      }
      if(orientation < 0.0f) {
        transform.localScale = new Vector3(
          -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
      }

      // Update animation
      animator.SetBool(GROUNDED, groundSensor.isStay());
      animator.SetFloat(H_SPEED, Mathf.Abs(rb2d.velocity.x));
      animator.SetFloat(V_SPEED, rb2d.velocity.y);
      animator.SetBool(DRAWING, isDrawing);
    }


    private float getOrientation() {
      return rb2d.velocity.x;
    }
  }
}

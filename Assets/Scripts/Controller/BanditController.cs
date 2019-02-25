using System.Collections;
using UnityEngine;

using Catsland.Scripts.Bullets;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  [RequireComponent(typeof(IInput))]
  [RequireComponent(typeof(Animator))]
  public class BanditController: MonoBehaviour {

    // Locomoation
    public float maxRunningSpeed = 1.0f;
    public float acceleration = 1.0f;

    // Attack
    public int damageValue = 1;
    public float chopingCd = 1.0f;
    private bool isChopping = false;

    // Health
    public int maxHealth = 3;
    public int currentHealth;
    public float dizzyTime = 1.0f;
    private bool isDead = false;
    private bool isDizzy = false;

    public float dieAnimationSecond = 1.0f;

    // References
    public GameObject groundSensorGO;
    public Melee melee;
    private ISensor groundSensor;
    private IInput input;
    private Rigidbody2D rb2d;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Animation
    private const string H_SPEED = "HSpeed";
    private const string DIZZY = "Dizzy";
    private const string IS_CHOPPING = "IsChopping";
    private const string IS_DEAD = "IsDead";

    public void Awake() {
      input = GetComponent<IInput>();
      rb2d = GetComponent<Rigidbody2D>();
      groundSensor = groundSensorGO.GetComponent<ISensor>();
      animator = GetComponent<Animator>();
      spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Start() {
      currentHealth = maxHealth;
    }

    public void Update() {
      float desiredSpeed = input.getHorizontal();
      float currentVerticleVolocity = rb2d.velocity.y;

      if(input.attack() && !isDizzy && !isChopping) {
        // Start chopping
        StartCoroutine(chop());
      }

      // Movement
      // horizontal movement
      gameObject.transform.parent =
        groundSensor.isStay() ? Utils.getAnyFrom(groundSensor.getTriggerGos()).transform : null;
      if(!isDizzy) {
        if(Mathf.Abs(desiredSpeed) > Mathf.Epsilon && groundSensor.isStay() && !isChopping) {
          rb2d.AddForce(new Vector2(acceleration * desiredSpeed, 0.0f));
          rb2d.velocity = new Vector2(
            Mathf.Clamp(rb2d.velocity.x, -maxRunningSpeed, maxRunningSpeed),
            rb2d.velocity.y);
        } else {
          rb2d.velocity = new Vector2(0.0f, rb2d.velocity.y);
        }
      } else {
        // dizzy, damp on horizontal speed
        rb2d.velocity =
          new Vector2(rb2d.velocity.x * 0.8f, rb2d.velocity.y);
      }

      // Update facing
      if(Mathf.Abs(desiredSpeed) > Mathf.Epsilon && !isDizzy) {
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
      animator.SetFloat(H_SPEED, Mathf.Abs(rb2d.velocity.x));
      animator.SetBool(DIZZY, isDizzy);
      animator.SetBool(IS_CHOPPING, isChopping);
      animator.SetBool(IS_DEAD, isDead);
    }

    public void damage(DamageInfo damageInfo) {
      rb2d.AddForce(damageInfo.repelDirection * damageInfo.repelIntense);
      currentHealth -= damageInfo.damage;
      if(currentHealth <= 0) {
        // Die
        StartCoroutine(die());
      } else {
        // Dizzy
        StartCoroutine(dizzy());
      }
    }

    public float getOrientation() {
      return transform.lossyScale.x > 0.0f ? 1.0f : -1.0f;
    }

    private IEnumerator chop() {
      isChopping = true;
      melee.turnOn(new DamageInfo(damageValue, Vector2.zero, new Vector2(getOrientation(), 0.0f), 1.0f));
      yield return new WaitForSeconds(chopingCd);
      melee.turnOff();
      isChopping = false;
    }

    private IEnumerator dizzy() {
      isDizzy = true;
      yield return new WaitForSeconds(dizzyTime);
      isDizzy = false;
    }

    private IEnumerator die() {
      isDizzy = true;
      isDead = true;
      yield return new WaitForSeconds(dieAnimationSecond);
      Destroy(gameObject);
    }
  }
}

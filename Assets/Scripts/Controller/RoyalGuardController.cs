using System.Collections.Generic;
using System;
using UnityEngine;
using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.Controller {
  public class RoyalGuardController: MonoBehaviour {

    public interface RoyalGuardInput {
      float getHorizontal();
      /*
      bool charge();
      bool jumpSmash();
      bool spell();
      */
      bool chop();
      bool jumpSmash();
      bool summon();
    }

    public enum Status {
      IDEAL = 1,

      // Chop
      CHOPPING = 2,

      JUMPING = 3,

      SUMMONING = 4,
    }

    public Status status = Status.IDEAL;

    // Walk
    public float walkingSpeed = 2.0f;

    // Chop
    public float chopMoveForwardDistance = 0.6f;
    public GameObject chopEffectGo;
    public Transform chopEffectGeneratePosition;
    public float chopEffectSpeed = 8.0f;
    private bool wantNextChop = false;

    // Charge
    public float chargeSpeed = 5.0f;
    public float chargePrepareTime = 0.5f;
    public float chargeChargingTime = 2.0f;
    public float chargeRestTime = 1.0f;

    // Jump Smash
    public float jumpDirectionDegree = 60.0f;
    public float jumpForce = 200.0f;
    public float smashEffectSpeed = 12.0f;
    public GameObject smashEffectGo;
    public Transform smashEffectoGeneratePosition;
    public Transform smashEffectHorizon;
    public GameObject smashSplashGo;

    public Vector2 jumpSmashJumpForce = new Vector2(50.0f, 200.0f);
    public float jumpSmashPrepareTime = 0.3f;
    public float jumpSmashSmashTime = 0.3f;
    public float jumpSmashRestTime = 0.5f;
    public GameObject jumpSmashEffectPrefab;
    public Transform jumpSmashEffectTransform;
    private bool isLastOnGround = false;

    // Spell
    public GameObject throwingKnifePrefab;
    public Transform knifeGenerationPoint;
    public float knifeSpeed = 8.0f;
    public float knifeAngularSpeed = 360.0f;
    public float spellPrepareTime = 0.3f;
    public float spellSpellTime = 0.3f;
    public float spellRestTime = 0.2f;
    public Transform spellTransform;


    // Aoe
    public int lineNumber = 5;
    public Transform lineTopPosition;
    public Transform lineBottomPosition;
    public float rangeWidth = 8.0f;
    public float minWidth = 0.5f;
    private GameObject lineParent;
    public GameObject warningLineGo;
    public GameObject aoeSpellGo;
    public float aoeSpellSpeed;
    public float aoeSpellHeightRange = 0.5f;
    public float noHaveLineWithinRange = 0.5f;

    private float currentChargeStatusRemainingTime;
    private LinearSequence chargeSequence;
    private LinearSequence jumpSmashSequence;
    private LinearSequence spellSequence;
    private LinearSequence dieSequence;

    // Die
    public float throwForceOnDie = 10.0f;
    public GameObject dieEffectGoPrefab;
    public Transform hurtEffectPoint;

    // Health
    public int maxHealth = 5;
    private int curHealth = 5;

    // References
    public GameObject groundSensorGo;
    private ISensor groundSensor;
    private Rigidbody2D rb2d;
    private RoyalGuardInput input;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Animation
    /*
    private static readonly string H_SPEED = "HSpeed";
    private static readonly string V_SPEED = "VSpeed";
    private static readonly string JUMP_SMASH_PHASE = "JumpSmashPhase";
    private static readonly string CHARGE_PHASE = "ChargePhase";
    private static readonly string SPELL_PHASE = "SpellPhase";
    private static readonly string DIE = "Die";
    */
    private static readonly string WANT_CHOP = "WantChop";
    private static readonly string WANT_JUMP = "WantJump";
    private static readonly string IS_GROUNDED = "IsGrounded";
    private static readonly string V_SPEED = "VSpeed";
    private static readonly string WANT_SUMMON = "WantSummon";

    private static readonly Dictionary<Status, int> JUMP_SMASH_STATUS_TO_PHASE =
      new Dictionary<Status, int>();
    private static readonly Dictionary<Status, int> CHARGE_STATUS_TO_PHASE =
      new Dictionary<Status, int>();
    private static readonly Dictionary<Status, int> SPELL_STATUS_TO_PHASE =
      new Dictionary<Status, int>();

    private static HashSet<Status> lockVelocityStatus = new HashSet<Status>();

    private static System.Random random = new System.Random();

    static RoyalGuardController() {
      lockVelocityStatus.Add(Status.IDEAL);
      lockVelocityStatus.Add(Status.CHOPPING);
      /*
      JUMP_SMASH_STATUS_TO_PHASE.Add(Status.JUMP_SMASH_PREPARE, 1);
      JUMP_SMASH_STATUS_TO_PHASE.Add(Status.JUMP_SMASH_JUMPING, 2);
      JUMP_SMASH_STATUS_TO_PHASE.Add(Status.JUMP_SMASH_SMASHING, 3);
      JUMP_SMASH_STATUS_TO_PHASE.Add(Status.JUMP_SMASH_REST, 4);

      CHARGE_STATUS_TO_PHASE.Add(Status.CHARGE_PREPARE, 1);
      CHARGE_STATUS_TO_PHASE.Add(Status.CHARGE_CHARGING, 2);
      CHARGE_STATUS_TO_PHASE.Add(Status.CHARGE_REST, 3);

      SPELL_STATUS_TO_PHASE.Add(Status.SPELL_PREAPRE, 1);
      SPELL_STATUS_TO_PHASE.Add(Status.SPELL_SPELLING, 2);
      SPELL_STATUS_TO_PHASE.Add(Status.SPELL_REST, 3);
      */
    }


    void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
      input = GetComponent<RoyalGuardInput>();
      groundSensor = groundSensorGo.GetComponent<ISensor>();
      animator = GetComponent<Animator>();
      spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
      curHealth = maxHealth;
    }

    void Update() {
      /*
      Status oldStatus = status;
      // autonamous logic
      status = (Status)chargeSequence.processIfInInterestedStatus(status);
      status = (Status)jumpSmashSequence.processIfInInterestedStatus(status);
      status = (Status)spellSequence.processIfInInterestedStatus(status);
      status = (Status)dieSequence.processIfInInterestedStatus(status);
      */

      // transition logic
      float desiredSpeed = input.getHorizontal();
      /*
      if(canCharge() && input.charge()) {
        status = (Status)chargeSequence.start();
      }
      if(canJumpSmash() && input.jumpSmash()) {
        status = (Status)jumpSmashSequence.start();
      }
      if(canSpell() && input.spell()) {
        status = (Status)spellSequence.start();
      }
      */
      if(canAdjustOrientation()) {
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
      // Chop
      if(status == Status.CHOPPING && input.chop()) {
        wantNextChop = true;
      }
      // Jump
      if(status == Status.IDEAL) {
        animator.SetBool(WANT_JUMP, input.jumpSmash());
        animator.SetBool(WANT_SUMMON, input.summon());
      }
      /*
      // apply velocity
      if(status == Status.CHARGE_CHARGING) {
        rb2d.velocity = new Vector2(getOrientation() * chargeSpeed, rb2d.velocity.y);
      } else if(status == Status.JUMP_SMASH_JUMPING) {
        if(oldStatus != status) {
          rb2d.velocity = Vector2.zero;
          rb2d.AddForce(new Vector2(getOrientation() * jumpSmashJumpForce.x, jumpSmashJumpForce.y));
        }
      } else if(oldStatus != Status.SPELL_SPELLING && status == Status.SPELL_SPELLING) {
        spell();
      } else if(canWalk()) {
        rb2d.velocity = new Vector2(desiredSpeed * walkingSpeed, rb2d.velocity.y);
      } else if(status == Status.DIE_THROW) {
        // do nothing on die throwing
        // fix on ground on die stay
      } else if(groundSensor.isStay()) {
        rb2d.velocity = new Vector2(0.0f, rb2d.velocity.y);
      }

      // effect
      if(!isLastOnGround && groundSensor.isStay()
        && (status == Status.JUMP_SMASH_JUMPING || status == Status.JUMP_SMASH_SMASHING)) {
        GameObject jumpSmashEffect = Instantiate(jumpSmashEffectPrefab);
        jumpSmashEffect.transform.position = jumpSmashEffectTransform.position;
        Utils.setRelativeRenderLayer(spriteRenderer, jumpSmashEffect.GetComponent<SpriteRenderer>(), 1);
      }
      isLastOnGround = groundSensor.isStay();

      if(oldStatus == Status.DIE_THROW && status == Status.DIE_STAY) {
        if(dieEffectGoPrefab != null) {
          GameObject dieEffectGo = Instantiate(dieEffectGoPrefab);
          dieEffectGo.transform.position = hurtEffectPoint.position;
        }
      }
      */

      // animation
      /*
      animator.SetFloat(H_SPEED, Mathf.Abs(rb2d.velocity.x));
      animator.SetFloat(V_SPEED, rb2d.velocity.y);
      setAnimiatorPhaseValue(JUMP_SMASH_PHASE, JUMP_SMASH_STATUS_TO_PHASE);
      setAnimiatorPhaseValue(CHARGE_PHASE, CHARGE_STATUS_TO_PHASE);
      setAnimiatorPhaseValue(SPELL_PHASE, SPELL_STATUS_TO_PHASE);
      animator.SetBool(DIE, isDead());
      */

      if(lockVelocityStatus.Contains(status)) {
        rb2d.velocity = Vector2.zero;
      }

      animator.SetBool(WANT_CHOP, input.chop() || wantNextChop);
      animator.SetBool(IS_GROUNDED, groundSensor.isStay());
      animator.SetFloat(V_SPEED, rb2d.velocity.y);

    }

    public float getOrientation() {
      return transform.lossyScale.x > 0.0f ? 1.0f : -1.0f;
    }

    public bool canAdjustOrientation() {
      return status == Status.IDEAL;
    }

    public bool canCharge() {
      return status == Status.IDEAL;
    }

    public bool canJumpSmash() {
      return status == Status.IDEAL;
    }

    public bool canSpell() {
      return status == Status.IDEAL;
    }

    public void damage(DamageInfo damageInfo) {
      if(isDead()) {
        return;
      }
      curHealth -= damageInfo.damage;
      if(curHealth <= 0) {
        enterDie();
      }
    }

    public bool isDead() {
      return curHealth <= 0;
    }

    private void enterDie() {
      ContactDamage[] contactDamages = GetComponentsInChildren<ContactDamage>();
      foreach(ContactDamage contactDamage in contactDamages) {
        contactDamage.enabled = false;
      }

      rb2d.velocity = Vector2.zero;
      rb2d.AddForce(new Vector2(-getOrientation() * throwForceOnDie, throwForceOnDie));

      status = (Status)dieSequence.start();

    }

    private bool canWalk() {
      return status == Status.IDEAL;
    }

    private void spell() {
      GameObject knife = Instantiate(throwingKnifePrefab);
      knife.transform.position = spellTransform.position;
      knife.transform.localScale = new Vector2(getOrientation(), 1.0f);
      // renderer
      SpriteRenderer knifeRenderer = knife.GetComponent<SpriteRenderer>();
      Utils.setRelativeRenderLayer(spriteRenderer, knifeRenderer, 1);

      // velocity
      Rigidbody2D knifeRb2d = knife.GetComponent<Rigidbody2D>();
      knifeRb2d.velocity = new Vector2(getOrientation() * knifeSpeed, 0.0f);

      knife.GetComponent<Spell>().fire(gameObject);
    }

    private void setAnimiatorPhaseValue(String variableName, Dictionary<Status, int> statusToPhase) {
      int phase = 0;
      statusToPhase.TryGetValue(status, out phase);
      animator.SetInteger(variableName, phase);
    }

    public void startChop() {
      status = Status.CHOPPING;
      wantNextChop = false;
      generateChopEffect();
    }

    private void generateChopEffect() {
      GameObject chop = Instantiate(chopEffectGo);
      chop.transform.position = chopEffectGeneratePosition.position;
      chop.transform.localScale = new Vector3(
        getOrientation() > 0.0f ? chop.transform.lossyScale.x : -chop.transform.lossyScale.x,
        chop.transform.lossyScale.y,
        chop.transform.lossyScale.z);

      chop.GetComponent<Rigidbody2D>().velocity = new Vector2(chopEffectSpeed * getOrientation(), 0.0f);
      chop.GetComponentInChildren<Spell>().fireWithSpecificRepel(gameObject, getOrientation());
    }

    public void makeOffset() {
      transform.position += new Vector3(chopMoveForwardDistance * getOrientation(), 0.0f);
    }

    public void startIdeal() {
      status = Status.IDEAL;
      if(lineParent != null) {
        cancelWarningLine();
      }
    }

    public void startJump() {
      Vector2 jumpVector =
        new Vector2(
        getOrientation() * Mathf.Cos(jumpDirectionDegree * Mathf.Deg2Rad),
        Mathf.Sin(jumpDirectionDegree * Mathf.Deg2Rad))
        * jumpForce;
      rb2d.AddForce(jumpVector);
      status = Status.JUMPING;
    }

    public void generateSmashEffect(Vector3 position, bool forward) {
      // smash attack
      GameObject smash = Instantiate(smashEffectGo);
      smash.transform.position = new Vector3(
        position.x,
        smashEffectHorizon.position.y,
        position.z);
      smash.transform.localScale = new Vector3(
        (forward ? 1 : -1) * getOrientation() * smash.transform.lossyScale.x,
        smash.transform.lossyScale.y,
        smash.transform.lossyScale.z);
      smash.GetComponent<Rigidbody2D>().velocity =
        new Vector2(smashEffectSpeed * getOrientation() * (forward ? 1 : -1), 0.0f);
      smash.GetComponentInChildren<Spell>().fireWithSpecificRepel(gameObject, getOrientation());
    }

    public void generateSingleSmashEfect() {
      generateSmashEffect(smashEffectoGeneratePosition.position, true);
    }

    public void generateDoubleSmashEffect() {
      generateSmashEffect(transform.position, true);
      generateSmashEffect(transform.position, false);
    }

    public void generateSmashSpash() {
      GameObject splash = Instantiate(smashSplashGo);
      splash.transform.position = new Vector3(
        transform.position.x,
        smashEffectHorizon.position.y,
        transform.position.z);
    }

    public void prepareAoe() {
      if(lineParent == null) {
        lineParent = new GameObject("WarningLine");
      }

      float aoeRangeLeft = transform.position.x - rangeWidth * 0.5f;
      float aoeRangeRight = transform.position.x + rangeWidth * 0.5f;
      float avgRange = rangeWidth / lineNumber;

      float x = aoeRangeLeft;
      for(int i = 0; i < lineNumber; i++) {
        float spellRangeRight = aoeRangeLeft + (i + 1) * avgRange;
        x = UnityEngine.Random.Range(x + minWidth, spellRangeRight);
        if(Mathf.Abs(x - transform.position.x) < noHaveLineWithinRange) {
          continue;
        }
        GameObject warningLine = Instantiate(warningLineGo);
        warningLine.transform.parent = lineParent.transform;
        warningLine.GetComponent<LineRenderer>().SetPositions(new[]{
          new Vector3(x, lineTopPosition.position.y),
          new Vector3(x, lineBottomPosition.position.y)});
      }
    }

    public void performAoe() {
      Debug.Log("Perform Aoe.");
      if(lineParent == null) {
        return;
      }
      foreach(LineRenderer lineRenderer in lineParent.transform.GetComponentsInChildren<LineRenderer>()) {
        Debug.Log("Create spell");
        Vector3 startPoint = lineRenderer.GetPosition(0);
        GameObject spell = Instantiate(aoeSpellGo);
        spell.transform.position =
          startPoint + new Vector3(0.0f, UnityEngine.Random.Range(0.0f, aoeSpellHeightRange), 0.0f);
        spell.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, -aoeSpellSpeed);
        spell.GetComponentInChildren<Spell>().fire(gameObject);
      }
      cancelWarningLine();
    }

    private void cancelWarningLine() {
      Destroy(lineParent);
    }
  }
}

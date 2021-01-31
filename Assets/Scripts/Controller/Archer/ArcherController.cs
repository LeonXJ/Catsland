using UnityEngine;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Common;
using Catsland.Scripts.Misc;
using Catsland.Scripts.Ui;

namespace Catsland.Scripts.Controller.Archer {
  public class ArcherController : MonoBehaviour, IHealthBarQuery {

    [Header("Basic")]
    public Timing timing;
    public string displayName = "Archer";

    [Header("Draw")]
    public float DrawTime = 1f;
    private float currentDrawTime = 0f;
    public float DrawIntense => Mathf.Clamp01(currentDrawTime / DrawTime);

    public float aimSpeed = 5f;
    private float aimDirection = .5f;

    public float aimDirectionRange = 60f;

    [Header("Arrow")]
    public GameObject arrowPrefab;
    public Transform arrowShootPoint;
    public float arrowSpeed = 10f;
    public float arrowLifetime = 3f;
    public Party.WeaponPartyConfig weaponPartyConfig;

    [Header("Health")]
    public VulnerableAttribute vulnerableAttribute;
    private Bullets.Utils.DamageHelper damageHelper;

    public DamageBypass bodyDamagePart;
    public DamageBypass headDamagePart;

    // Whether the last damage is headshot. Used to determine what debris to generate.
    private bool wasHeadShot = false;

    [Header("Debris")]
    public DebrideGenerator normalDebrisGenerator;
    public DebrideGenerator headshotDebrisGenerator;

    // References
    public Transform upper;
    private ArcherEventSounds eventSounds;

    private IInput input;
    private Animator animator;
    private DrawAnimation drawAnimation;

    private const string ANI_STAND = "Archer_Stand";
    private const string ANI_LOWER_DRAW = "Archer_Lower_Draw";

    private const string PAR_IS_DRAWING = "IsDrawing";
    private const string PAR_HAS_ENEGY_TO_SHOOT = "HasEnegyToShoot";
    private const string PAR_UPPER_DIRECTION = "UpperDirection";

    // Start is called before the first frame update
    void Start() {
      input = GetComponent<IInput>();
      animator = GetComponent<Animator>();
      drawAnimation = GetComponent<DrawAnimation>();
      eventSounds = GetComponent<ArcherEventSounds>();

      damageHelper = Bullets.Utils.DamageHelper.DamageHelperBuilder.NewBuilder(
        this, vulnerableAttribute, timing)
        .SetOnDie(EnterDie)
        .Build();
    }

    // Update is called once per frame
    void Update() {
      bool isDrawing = false;
      if (input.attack() && canDraw()) {
        currentDrawTime += Time.deltaTime;
        isDrawing = true;
        aimPlayer();
      }

      if (shouldReleaseDrawEnegy(isDrawing)) {
        currentDrawTime = 0f;
        aimDirection = .5f;
      }

      if (canCanOrientation()) {
        ControllerUtils.AdjustOrientation(input.getHorizontal(), gameObject);
      }

      // Update animator
      animator.SetBool(PAR_IS_DRAWING, isDrawing);
      animator.SetBool(PAR_HAS_ENEGY_TO_SHOOT, currentDrawTime > DrawTime);
      animator.SetFloat(PAR_UPPER_DIRECTION, aimDirection);

      drawAnimation.InEffect = isDrawing;
      drawAnimation.Intensity = Mathf.Clamp01(currentDrawTime / DrawTime);
    }

    public void Shoot() {
      GameObject arrow = Instantiate(arrowPrefab);
      arrow.transform.position = arrowShootPoint.position;

      Vector2 arrowVelocity = arrowSpeed * 
        (GetOrientation() > 0 ? 1f : -1f) * arrowShootPoint.transform.right;

      ArrowCarrier arrowCarrier = arrow.GetComponent<ArrowCarrier>();
      arrowCarrier.fire(arrowVelocity, arrowLifetime, gameObject.tag, weaponPartyConfig);
    }

    public bool IsShootBlock() {
      GameObject player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      if (player == null) {
        return true;
      }

      Vector2 ray = player.transform.position - arrowShootPoint.position;
      var hit =Physics2D.Raycast(arrowShootPoint.position, ray, ray.magnitude, LayerMask.GetMask(Layers.LAYER_GROUND_NAME));
      return hit.collider != null;
    }

    private void aimPlayer() {
      // Find player
      GameObject player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      if (player == null) {
        aimDirection = .5f;
        return;
      }
      Vector2 delta = player.transform.position - upper.position;

      // Cap delta with aim direction range
      float proposeAimDirection = Mathf.Clamp(
        Vector2.SignedAngle(new Vector2(GetOrientation(), 0f), delta),
        -aimDirectionRange, aimDirectionRange) * .5f / aimDirectionRange + .5f;
      // Reverse if faces left.
      if (GetOrientation() < 0f) {
        proposeAimDirection = 1f - proposeAimDirection;
      }

      // Lerp by aim speed
      aimDirection = Mathf.LerpAngle(aimDirection, proposeAimDirection, Time.deltaTime * aimSpeed);
    }

    public float GetOrientation() {
      return transform.lossyScale.x > 0f ? 1f : -1f;
    }

    // Damage cause by melee. 
    public void damage(DamageInfo damageInfo) {
      eventSounds?.PlayOnDamageSound();
      damageHelper.OnDamaged(damageInfo);
    }

    // Arrow damage bypassed from sub-component
    public void OnDamageByPass(DamageBypassInfo damageBypassInfo) {
      eventSounds?.PlayOnDamageSound();
      int damageMultiplier = 1;
      // Headshot: damage x 2
      if (damageBypassInfo.byPasser == headDamagePart.gameObject.name) {
        damageMultiplier = 2;
        wasHeadShot = true;
      } else {
        wasHeadShot = false;
      }
      damageHelper.OnDamaged(damageBypassInfo.damageInfo, damageMultiplier);
    }

    private bool canCanOrientation() {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return baseState.IsName(ANI_STAND) || baseState.IsName(ANI_LOWER_DRAW);
    }

    private bool canDraw() {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return baseState.IsName(ANI_STAND) || baseState.IsName(ANI_LOWER_DRAW);
    }

    private bool shouldReleaseDrawEnegy(bool isDrawing) {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return !isDrawing && baseState.IsName(ANI_STAND);
    }

    private void EnterDie(DamageInfo damageInfo) {
      eventSounds?.PlayOnDieSound();
      if (wasHeadShot) {
        headshotDebrisGenerator?.GenerateDebrides(damageInfo.repelDirection);
      } else {
        normalDebrisGenerator?.GenerateDebrides(damageInfo.repelDirection);
      }
      Destroy(gameObject);
    }

    public HealthCondition GetHealthCondition() {
      return HealthCondition.CreateHealthCondition(vulnerableAttribute, displayName);
    }
  }
}

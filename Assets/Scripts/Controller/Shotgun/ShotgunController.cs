using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Misc;
using Catsland.Scripts.Ui;

namespace Catsland.Scripts.Controller.Shotgun {
  public class ShotgunController : MonoBehaviour, IHealthBarQuery {

    // Animation state names.
    private const string ANI_IDLE = "Shotgun_Idle";
    private const string ANI_WALK = "Shotgun_Walk";
    private const string ANI_SHOOT= "Shotgun_Shoot";
    private const string ANI_RELOAD = "Shotgun_Reload";

    // Animation parameters.
    private const string PAR_IS_WALKING = "IsWalking";
    private const string PAR_WANT_SHOOT = "WantShoot";

    [Header("Basic")]
    public Timing timing;
    public string displayName = "Shotgun";

    [Header("Walk")]
    public float walkSpeed = 1f;
    private float walkSpeedScale = 1f;

    [Header("Shoot")]
    public ParticleSystem gunfireParticle;
    public ParticleSystem shellParticle;

    [Header("Health")]
    public VulnerableAttribute vulnerableAttribute;
    private Bullets.Utils.DamageHelper damageHelper;

    private DebrideGenerator debrideGenerator;

    public DamageBypass weakPointDamagePart;

    // References
    private Rigidbody2D rb2d;
    private IInput input;
    private Animator animator;


    // Start is called before the first frame update
    void Start() {
      rb2d = GetComponent<Rigidbody2D>();
      input = GetComponent<IInput>();
      animator = GetComponent<Animator>();
      debrideGenerator = GetComponent<DebrideGenerator>();

      damageHelper = Bullets.Utils.DamageHelper.DamageHelperBuilder.NewBuilder(
        this, vulnerableAttribute, timing)
        .SetOnDie(EnterDie)
        .Build();
    }

    // Update is called once per frame
    void Update() {

      int wantOrientation = Common.Utils.IntSignWithZero(input.getHorizontal());

      // Shot override walk
      bool wantShoot = input.attack();

      if (CanChangeOrientation() && wantOrientation != 0) {
        ControllerUtils.AdjustOrientation(wantOrientation, gameObject);
      }

      bool isWalking = false;
      if (CanDetermineVelocity()) {
        if (CanWalk(wantShoot)) {
          rb2d.velocity = new Vector2(wantOrientation * walkSpeed * walkSpeedScale, rb2d.velocity.y);
          isWalking = wantOrientation != 0;
        } else {
          rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
        }
      }

      // Update animiation parameters.
      animator.SetBool(PAR_IS_WALKING, isWalking);
      animator.SetBool(PAR_WANT_SHOOT, wantShoot);
    }

    public void GenerateGunfileParticle() {
      gunfireParticle?.Play(true);
    }

    public void GenerateShell() {
      shellParticle?.Play();
    }

    public void SetWalkSpeedScale(float scale) {
      walkSpeedScale = scale;
    }

    public float GetOrientation() {
      return transform.lossyScale.x > 0f ? 1f : -1f;
    }

    public void OnDamageByPass(DamageBypassInfo damageBypassInfo) {
      if (damageBypassInfo.byPasser == weakPointDamagePart.name) {
        damageHelper.OnDamaged(damageBypassInfo.damageInfo, 1);
      }
    }
    private void EnterDie(DamageInfo damageInfo) {
      debrideGenerator?.GenerateDebrides();
      Destroy(gameObject);
    }

    // Whether the the velocity is under control. Otherwise be subject to inertia.
    private bool CanDetermineVelocity() {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return !baseState.IsName(ANI_SHOOT) && !baseState.IsName(ANI_RELOAD);
    }

    private bool CanWalk(bool wantShoot) {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return !wantShoot && (baseState.IsName(ANI_IDLE) || baseState.IsName(ANI_WALK));
    }

    private bool CanChangeOrientation() {
      AnimatorStateInfo baseState = animator.GetCurrentAnimatorStateInfo(0);
      return !baseState.IsName(ANI_SHOOT) && !baseState.IsName(ANI_RELOAD);
    }

    public HealthCondition GetHealthCondition() {
      return HealthCondition.CreateHealthCondition(vulnerableAttribute, displayName);
    }
  }
}

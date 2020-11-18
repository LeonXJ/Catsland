using UnityEngine;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Ui;
using Catsland.Scripts.Misc;

namespace Catsland.Scripts.Controller.Crawler {
  [RequireComponent(typeof(Rigidbody2D)),
    RequireComponent(typeof(CrawlerInput)),
    RequireComponent(typeof(Animator))]
  public class CrawlerController : MonoBehaviour, IHealthBarQuery {

    public interface CrawlerInput {
      float GetHorizontal();
    }

    public string displayName = "Crawler";
    public float moveSpeed = 1f;
    public VulnerableAttribute vulnerableAttribute;

    public Timing timing;

    private bool isDizzy = false;

    private CrawlerInput input;
    private Rigidbody2D rb2d;
    private Animator animator;
    private Bullets.Utils.DamageHelper damageHelper;
    private DebrideGenerator debrideGenerator;


    // Start is called before the first frame update
    void Start() {
      input = GetComponent<CrawlerInput>();
      rb2d = GetComponent<Rigidbody2D>();
      animator = GetComponent<Animator>();
      debrideGenerator = GetComponent<DebrideGenerator>();

      damageHelper = Utils.DamageHelper.DamageHelperBuilder.NewBuilder(
        this, vulnerableAttribute, timing)
        .SetOnDie(EnterDie)
        .Build();
    }

    // Update is called once per frame
    void Update() {
      int wantOrientation = Common.Utils.IntSignWithZero(input.GetHorizontal());
      if (CanChangeOrientation() && wantOrientation != 0) {
        ControllerUtils.AdjustOrientation(wantOrientation, gameObject);
      }

      if (CanMove()) {
        rb2d.velocity = new Vector2(wantOrientation * moveSpeed, rb2d.velocity.y);
      }
    }
    public void damage(DamageInfo damageInfo) {
      damageHelper.OnDamaged(damageInfo);
    }

    private void EnterDie(DamageInfo damageInfo) {
      /*
      if (dieEffectPrefab != null) {
        GameObject dieEffect = Instantiate(dieEffectPrefab);
        dieEffect.transform.position = Vector3Builder.From(transform.position).SetZ(AxisZ.SPLASH).Build();
        dieEffect.GetComponent<ParticleSystem>()?.Play(true);
      }
      */

      // slimeEventSounds?.PlayOnDieSound();
      // diamondGenerator?.GenerateDiamond();
      debrideGenerator?.GenerateDebrides(damageInfo.repelDirection);
      Destroy(gameObject);
    }


    private bool CanChangeOrientation() {
      return !isDizzy;
    }

    private bool CanMove() {
      return !isDizzy;
    }

    public HealthCondition GetHealthCondition() {
      return HealthCondition.CreateHealthCondition(vulnerableAttribute, displayName);
    }
  }
}

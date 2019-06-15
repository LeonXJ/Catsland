using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Misc;
using static Catsland.Scripts.Bullets.Utils;
using System.Collections;

namespace Catsland.Scripts.Controller {
  [RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Animator)), RequireComponent(typeof(CaterpillarController))]
  public class CaterpillarController : MonoBehaviour {

    public enum Status {
      IDEAL = 0,
      RUNNING = 1,
      DIE = 2,
      DIZZY = 3,
    }

    public interface CaterpillarInput {
      float getHorizontal();
    }

    public float runningSpeed = 0.5f;
    public int maxHp = 3;
    public Color camelflagColor;
    public float colorChangeSpeed = 0.1f;
    public float dizzyTimeInSecond = 0.5f;

    private Rigidbody2D rb2d;
    private Animator animator;
    private CaterpillarInput input;
    private Status status;
    private int currentHp;
    private static readonly Dictionary<Status, string> STATUS_MAP;
    private DiamondGenerator diamondGenerator;
    private SpriteRenderer renderer;
    private bool isDizzy = false;

    private static readonly string H_ABS_SPEED = "HAbsSpeed";
    private static readonly string IS_DIZZY = "IsDizzy";

    static CaterpillarController() {
      STATUS_MAP = new Dictionary<Status, string>();
      STATUS_MAP.Add(Status.IDEAL, "Ideal");
      STATUS_MAP.Add(Status.RUNNING, "Running");
      STATUS_MAP.Add(Status.DIE, "Die");
      STATUS_MAP.Add(Status.DIZZY, "Dizzy");
    }

    private void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
      animator = GetComponent<Animator>();
      input = GetComponent<CaterpillarInput>();
      diamondGenerator = GetComponent<DiamondGenerator>();
      renderer = GetComponent<SpriteRenderer>();
    }


    // Start is called before the first frame update
    void Start() {
      currentHp = maxHp;
    }

    // Update is called once per frame
    void Update() {

      Vector2 wantDirection = new Vector2(input.getHorizontal(), 0.0f).normalized;
      status = ControllerUtils.GetStatusFromAnimator(animator, STATUS_MAP, Status.IDEAL);

      if (status == Status.IDEAL) {
        renderer.material.SetColor("_Color", Color.Lerp(renderer.material.GetColor("_Color"), Color.black, colorChangeSpeed * Time.deltaTime));
        renderer.material.SetColor("_AmbientLight", Color.Lerp(renderer.material.GetColor("_AmbientLight"), camelflagColor, colorChangeSpeed * Time.deltaTime));
        rb2d.gravityScale = 0.0f;
        rb2d.velocity = Vector2.zero;
      } else {
        renderer.material.SetColor("_Color", Color.Lerp(renderer.material.GetColor("_Color"), Color.white, colorChangeSpeed * Time.deltaTime));
        renderer.material.SetColor("_AmbientLight", Color.Lerp(renderer.material.GetColor("_AmbientLight"), Color.black, colorChangeSpeed * Time.deltaTime));
        rb2d.gravityScale = 1.0f;
      }

      if (CanChangeOrientation()) {
        ControllerUtils.AdjustOrientation(wantDirection.x, gameObject);
      }

      if (CanMove()) {
        rb2d.velocity = new Vector2(wantDirection.x * runningSpeed, rb2d.velocity.y);
      }

      // Animation
      animator.SetFloat(H_ABS_SPEED, Mathf.Abs(rb2d.velocity.x));

    }

    public bool CanChangeOrientation() {
      return status == Status.IDEAL || status == Status.RUNNING;
    }

    public bool CanMove() {
      return (status == Status.IDEAL || status == Status.RUNNING) && !isDizzy;
    }

    public void damage(DamageInfo damageInfo) {
      if (currentHp <= 0) {
        return;
      }
      StartCoroutine(dizzy());
      ApplyRepel(damageInfo, rb2d);
      currentHp -= damageInfo.damage;
      if (currentHp <= 0) {
        enterDie();
        return;
      }
    }

    private IEnumerator dizzy() {
      animator.SetBool(IS_DIZZY, true);
      //isDizzy = true;
      yield return new WaitForSeconds(dizzyTimeInSecond);
      isDizzy = false;
      animator.SetBool(IS_DIZZY, false);
    }

    private void enterDie() {
      if (diamondGenerator != null) {
        diamondGenerator.Generate(3, 1);
      }
      Destroy(gameObject);
    }
  }
}

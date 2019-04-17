using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.Controller {
  [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Animator)), RequireComponent(typeof(CaterpillarController))]
  public class CaterpillarController: MonoBehaviour {

    public enum Status {
      IDEAL = 0,
      RUNNING = 1,
      DIE = 2,
    }

    public interface CaterpillarInput {
      float getHorizontal();
    }

    public float runningSpeed = 0.5f;
    public int maxHp = 3;

    private Rigidbody2D rb2d;
    private Animator animator;
    private CaterpillarInput input;
    private Status status;
    private int currentHp;
    private static readonly Dictionary<Status, string> STATUS_MAP;

    private static readonly string H_ABS_SPEED = "HAbsSpeed";

    static CaterpillarController() {
      STATUS_MAP = new Dictionary<Status, string>();
      STATUS_MAP.Add(Status.IDEAL, "Ideal");
      STATUS_MAP.Add(Status.RUNNING, "Running");
      STATUS_MAP.Add(Status.DIE, "Die");
    }

    private void Awake() {
      rb2d = GetComponent<Rigidbody2D>();
      animator = GetComponent<Animator>();
      input = GetComponent<CaterpillarInput>();
    }


    // Start is called before the first frame update
    void Start() {
      currentHp = maxHp;
    }

    // Update is called once per frame
    void Update() {

      Vector2 wantDirection = new Vector2(input.getHorizontal(), 0.0f).normalized;
      status = ControllerUtils.GetStatusFromAnimator(animator, STATUS_MAP, Status.IDEAL);

      if(CanChangeOrientation()) {
        ControllerUtils.AdjustOrientation(wantDirection.x, gameObject);
      }

      if(CanMove()) {
        rb2d.velocity = new Vector2(wantDirection.x * runningSpeed, rb2d.velocity.y);
      }

      // Animation
      animator.SetFloat(H_ABS_SPEED, Mathf.Abs(rb2d.velocity.x));

    }

    public bool CanChangeOrientation() {
      return status == Status.IDEAL || status == Status.RUNNING;
    }

    public bool CanMove() {
      return status == Status.IDEAL || status == Status.RUNNING;
    }

    public void damage(DamageInfo damageInfo) {
      if(currentHp <= 0) {
        return;
      }
      currentHp -= damageInfo.damage;
      if(currentHp <= 0) {
        enterDie();
      }
    }

    private void enterDie() {
      Destroy(gameObject);
    }
  }
}

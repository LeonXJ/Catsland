using UnityEngine;
using System.Collections;
using Panda;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Ai {
  public class BoarAi: MonoBehaviour, BoarController.BoarInput {

    public Vector2 detectRange = new Vector2(5.0f, 3.0f);

    private float wantHorizontal;
    private bool wantPrepare;
    private bool wantCharge;
    private bool wantThrowStone;

    private Utils utils;
    private BoarController controller;

    public bool charge() {
      return wantCharge;
    }

    public float getHorizontal() {
      return wantHorizontal;
    }

    public bool prepare() {
      return wantPrepare;
    }

    public bool throwStone() {
      return wantThrowStone;
    }

    [Task]
    public void isPlayerInDetectRange() {
      utils.setTaskSucceedIfPlayerInDistanceOrFail(detectRange);
    }

    [Task]
    public void doPrepare() {
      resetStatus();
      wantPrepare = true;
      Task.current.Succeed();
    }

    [Task]
    public void doCharge() {
      resetStatus();
      Utils.succeedOnConditionElseDo(controller.consumeHasCharge(),
        () => {
          wantPrepare = true;
          wantCharge = true;
        });
    }

    [Task]
    public void facePlayer() {
      resetStatus();
      float actual = Common.Utils.getOrientation(gameObject);
      float delta = utils.getDistanceToPlayer().x;

      Utils.succeedOnConditionElseDo(actual * delta > 0.0f,
        () => {
          wantHorizontal = Mathf.Sign(delta);
        });
    }

    [Task]
    public void doThrowStone() {
      resetStatus();
      Utils.succeedOnConditionElseDo(controller.consumeHasThrowStone(),
        () => {
          wantPrepare = true;
          wantThrowStone = true;
        });
    }

    void Awake() {
      utils = new Utils(gameObject);
      controller = gameObject.GetComponent<BoarController>();
    }


    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void resetStatus() {
      wantHorizontal = 0.0f;
      wantPrepare = false;
      wantCharge = false;
      wantThrowStone = false;
    }
  }
}

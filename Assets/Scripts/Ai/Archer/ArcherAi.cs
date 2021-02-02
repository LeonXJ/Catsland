using UnityEngine;
using Panda;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;
using Catsland.Scripts.Controller.Archer;

namespace Catsland.Scripts.Ai.Archer {
  public class ArcherAi : MonoBehaviour, IInput {

    private float horizon = 0f;
    private bool wantAttack = false;
    private bool wantInteract = false;

    [Header("Detect")]
    public float visionDistance = 10f;
    public float visionAngle = 75f;
    public float senseDistance = 5f;

    [Header("Shoot")]
    public float holdFullDrawTime = 1f;
    private float fullDrawAccumulateTime = 0f;

    // Reference
    private ArcherController controller;
    private GameObject trap;

    [Task]
    public void Idle() {
      wantAttack = false;
      wantInteract = false;
      Task.current.Succeed();
    }

    [Task]
    public void DetectPlayer() {
      GameObject player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      if (player == null) {
        Task.current.Fail();
        return;
      }
      Vector2 delta = player.transform.position - transform.position;

      // Out of range
      if (delta.sqrMagnitude > visionDistance * visionDistance) {
        Task.current.Fail();
        return;
      }

      // Inside sense range
      if (delta.sqrMagnitude < senseDistance * senseDistance) {
        Task.current.Succeed();
        return;
      }

      Vector2 orientation = new Vector2(controller.GetOrientation(), 0f);
      // Out of angle range
      if (Vector2.Angle(orientation, delta) > visionAngle) {
        Task.current.Fail();
        return;
      }
      Task.current.Succeed();
    }

    [Task]
    public void FacePlayer() {
      GameObject player = GameObject.FindGameObjectWithTag(Tags.PLAYER);
      if (player == null) {
        Task.current.Fail();
        return;
      }
      float deltaX = player.transform.position.x - transform.position.x;
      horizon = Mathf.Sign(deltaX);
      Task.current.Succeed();
    }

    [Task]
    public void AlreadyHasTrap() {
      if (controller.GetLastTrap() != null) {
        Task.current.Succeed();
        return;
      }
      Task.current.Fail();
    }


    [Task]
    public void SetTrap() {
      wantInteract = true;
      if (controller.IsSettingTrap()) {
        wantInteract = false;
        Task.current.Succeed();
        return;
      }
    }

    [Task]
    public void Draw() {
      if (controller.DrawIntense < 1f - Mathf.Epsilon) {
        wantAttack = true;
        fullDrawAccumulateTime = 0f;
        return;
      }
      // Full draw
      if (fullDrawAccumulateTime < holdFullDrawTime) {
        fullDrawAccumulateTime += Time.deltaTime;
        return;
      }
      // Full draw and hold enough time
      //wantAttack = false;
      // TODO: Figure out the NRE insdie of Task.current
      // No exception if: wantAttack = true
      //
      // As a walkaround, we split the Shoot() into Draw() and Release()
      Task.current.Succeed();
    }

    [Task]
    public void CanHitPlayer() {
      if (controller.IsShootBlock()) {
        Task.current.Fail();
        return;
      }
      Task.current.Succeed();
    }

    [Task]
    public void Release() {
      wantAttack = false;
      Task.current.Succeed();
    }

    public bool attack() {
      return wantAttack;
    }

    public bool dash() {
      throw new System.NotImplementedException();
    }

    public float getHorizontal() {
      return horizon;
    }

    public float getVertical() {
      return 0f;
    }

    public bool interact() {
      return wantInteract;
    }

    public bool jump() {
      return false;
    }

    public bool jumpHigher() {
      return false;
    }

    public bool meditation() {
      return false;
    }

    public bool timeSlow() {
      return false;
    }

    // Start is called before the first frame update
    void Start() {
      controller = GetComponent<ArcherController>();
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnDrawGizmos() {
      // Vision
      Common.Utils.DrawPieAsGizmos(
        visionDistance,
        Color.yellow,
        transform.position,
        new Vector2(Mathf.Sign(transform.lossyScale.x), 0f),
        visionAngle * 2f);

      // Sense
      Common.Utils.DrawCircleAsGizmos(
        senseDistance,
        Color.red,
        transform.position);
    }
  }
}

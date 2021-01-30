using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller.Archer {
  public class ArcherController : MonoBehaviour {


    [Header("Draw")]
    public float DrawTime = 1f;
    private float currentDrawTime = 0f;

    public float aimSpeed = 5f;
    private float aimDirection = .5f;

    public float aimDirectionRange = 60f;

    // References
    public Transform upper;

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
        Vector2.SignedAngle(new Vector2(getOrientation(), 0f), delta),
        -aimDirectionRange, aimDirectionRange) * .5f / aimDirectionRange + .5f;
      // Reverse if faces left.
      if (getOrientation() < 0f) {
        proposeAimDirection = 1f - proposeAimDirection;
      }

      // Lerp by aim speed
      aimDirection = Mathf.LerpAngle(aimDirection, proposeAimDirection, Time.deltaTime * aimSpeed);
    }

    private float getOrientation() {
      return transform.lossyScale.x > 0f ? 1f : -1f;
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
  }
}

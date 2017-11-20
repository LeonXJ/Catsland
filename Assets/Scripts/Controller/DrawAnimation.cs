using UnityEngine;

namespace Catsland.Scripts.Controller {

  [RequireComponent(typeof(SpriteRenderer))]
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(PlayerController))]
  public class DrawAnimation :MonoBehaviour {

    public string standDrawStateName = "draw";
    public Sprite[] standDrawSprites;

    public string jumpUpStateName = "jumpup";
    public Sprite[] jumpUpSprites;

    public string jumpDownStateName = "fall";
    public Sprite[] jumpDownSprites;

    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake() {
      spriteRenderer = GetComponent<SpriteRenderer>();
      playerController = GetComponent<PlayerController>();
      animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void LateUpdate() {
      ifStateThenUpdate(standDrawStateName, standDrawSprites);
      ifStateThenUpdate(jumpUpStateName, jumpUpSprites);
      ifStateThenUpdate(jumpDownStateName, jumpDownSprites);
    }

    bool ifStateThenUpdate(string stateName, Sprite[] sprites) {
      AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
      if (stateInfo.IsName(stateName)) {
        float intensity = playerController.getDrawIntensity();
        int index = (int)Mathf.Ceil(Mathf.Clamp(intensity, 0.0f, 1.0f) * (sprites.Length - 1));
        spriteRenderer.sprite = sprites[index];
        return true;
      }
      return false;
    }
  }

}

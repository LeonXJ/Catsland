using UnityEngine;

namespace Catsland.Scripts.Controller {

  [RequireComponent(typeof(SpriteRenderer))]
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(PlayerController))]
  public class DrawAnimation: MonoBehaviour {

    public string standDrawStateName = "draw";
    public Sprite[] standDrawSprites;

    public string jumpUpStateName = "jumpup";
    public Sprite[] jumpUpSprites;

    public string jumpDownStateName = "fall";
    public Sprite[] jumpDownSprites;

    [Header("Glow")]
    public SpriteRenderer glowSpriteRenderer;
    public float minGlowEffectIntensity = 0.5f;
    public float glowAlphaMin = 0.0f;
    public float glowAlphaMax = 0.8f;
    public float glowSizeMin = 0.2f;
    public float glowSizeMax = 2.0f;

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

      AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
      if (stateInfo.IsName(standDrawStateName) || stateInfo.IsName(jumpUpStateName) || stateInfo.IsName(jumpDownStateName)) {
        float intensity = playerController.getDrawIntensity();
        intensity = intensity < minGlowEffectIntensity ? 0.0f : (intensity - minGlowEffectIntensity) / (1.0f - minGlowEffectIntensity);
        glowSpriteRenderer.color = new Color(
          glowSpriteRenderer.color.r, glowSpriteRenderer.color.g, glowSpriteRenderer.color.b, Mathf.Lerp(glowAlphaMin, glowAlphaMax, intensity));
        float length = Mathf.Lerp(glowSizeMax, glowSizeMin, intensity);
        glowSpriteRenderer.transform.localScale = new Vector3(length, length, 1.0f);
      } else {
        glowSpriteRenderer.color = new Color(
          glowSpriteRenderer.color.r, glowSpriteRenderer.color.g, glowSpriteRenderer.color.b,
          Mathf.Lerp(glowSpriteRenderer.color.a, 0f, Time.deltaTime * 2.0f));
      }
    }

    bool ifStateThenUpdate(string stateName, Sprite[] sprites) {
      AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
      if(stateInfo.IsName(stateName)) {
        float intensity = playerController.getDrawIntensity();
        int index = (int)Mathf.Ceil(Mathf.Clamp(intensity, 0.0f, 1.0f) * (sprites.Length - 1));
        if(index == 0) {
          return false;
        }
        spriteRenderer.sprite = sprites[index];
        return true;
      }
      return false;
    }
  }

}

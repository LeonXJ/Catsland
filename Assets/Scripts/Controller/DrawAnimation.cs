using UnityEngine;

namespace Catsland.Scripts.Controller {

  [RequireComponent(typeof(SpriteRenderer))]
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(PlayerController))]
  public class DrawAnimation: MonoBehaviour {

    private static readonly string FULL_CHARGE_HALO_PARAMETER = "IsFullCharged";

    public string standDrawStateName = "draw";
    public Sprite[] standDrawSprites;

    public string jumpUpStateName = "jumpup";
    public Sprite[] jumpUpSprites;

    public string jumpDownStateName = "fall";
    public Sprite[] jumpDownSprites;

    [Header("Charging")]
    public SpriteRenderer glowSpriteRenderer;
    public float minGlowEffectIntensity = 0.5f;
    public float glowAlphaMin = 0.0f;
    public float glowAlphaMax = 0.8f;
    public float glowSizeMin = 0.2f;
    public float glowSizeMax = 2.0f;
    public ParticleSystem chargingParticle;

    private bool isPrevCharing = false;

    [Header("FullCharged")]
    public ParticleSystem fullChargedParticle;
    public Animator fullyChargedHalo;
    public ParticleSystem fullChargedShiningParticle;

    private bool isPrevFullCharged = false;

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

      bool isCurrentCharging = false;
      bool isCurrentFullCharged = false;
      AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
      if (stateInfo.IsName(standDrawStateName) || stateInfo.IsName(jumpUpStateName) || stateInfo.IsName(jumpDownStateName)) {
        float intensity = playerController.getDrawIntensity();
        intensity = intensity < minGlowEffectIntensity ? 0.0f : (intensity - minGlowEffectIntensity) / (1.0f - minGlowEffectIntensity);
        glowSpriteRenderer.color = new Color(
          glowSpriteRenderer.color.r, glowSpriteRenderer.color.g, glowSpriteRenderer.color.b, Mathf.Lerp(glowAlphaMin, glowAlphaMax, intensity));
        float length = Mathf.Lerp(glowSizeMax, glowSizeMin, intensity);
        glowSpriteRenderer.transform.localScale = new Vector3(length, length, 1.0f);
        if (intensity > 0.0f) {
          isCurrentCharging = true;
        }
        if (intensity >= 1.0f - Mathf.Epsilon) {
          isCurrentFullCharged = true;
        }
      } else {
        glowSpriteRenderer.color = new Color(
          glowSpriteRenderer.color.r, glowSpriteRenderer.color.g, glowSpriteRenderer.color.b,
          Mathf.Lerp(glowSpriteRenderer.color.a, 0f, Time.deltaTime * 2.0f));
      }

      // Update charging boolean
      if (isCurrentCharging && !isPrevCharing) {
        ReleaseCharingParticleEffect();
      }
      isPrevCharing = isCurrentCharging;

      // Update full charged boolean
      if (isCurrentFullCharged && !isPrevFullCharged) {
        ReleaseFullChargedParticleEffect();
      }
      isPrevFullCharged = isCurrentFullCharged;

      // Update full charged halo, shining
      SetFullChargedHalo(isCurrentFullCharged);
      SetFullChargedShining(isCurrentFullCharged);
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

    void ReleaseCharingParticleEffect() {
      if (chargingParticle != null) {
        chargingParticle.Play();
      }
    }

    void ReleaseFullChargedParticleEffect() {
      if (fullChargedParticle != null) {
        fullChargedParticle.Play();
      } 
    }

    void SetFullChargedHalo(bool isFullCharged) {
      if (fullyChargedHalo != null) {
        fullyChargedHalo.SetBool(FULL_CHARGE_HALO_PARAMETER, isFullCharged);
      }
    }

    void SetFullChargedShining(bool isFullCharged) {
      if (fullChargedShiningParticle != null) {
        if (isFullCharged) {
          ParticleSystem.EmissionModule emission = fullChargedShiningParticle.emission;
          emission.enabled = true;
        } else if (fullChargedShiningParticle.isPlaying){
          ParticleSystem.EmissionModule emission = fullChargedShiningParticle.emission;
          emission.enabled = false;
        }
      }
    }
  }
}

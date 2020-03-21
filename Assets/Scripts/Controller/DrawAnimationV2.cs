using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller {
  [ExecuteInEditMode]
  [RequireComponent(typeof(PlayerController), typeof(Animator))]
  public class DrawAnimationV2 : MonoBehaviour {

    public SpriteRenderer spriteRenderer;
    public string shootingStateName = "UpperBodyShoot";

    public Sprite[] shootingSprite;

    private PlayerController playerController;
    private Animator animator;

    // Start is called before the first frame update
    void Start() {
      playerController = GetComponent<PlayerController>();
      animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {

    }

    void LateUpdate() {
      AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(2);
      if (stateInfo.IsName(shootingStateName)) {
        float intensity = playerController.getDrawIntensity();
        int index = (int)Mathf.Ceil(Mathf.Clamp(intensity, 0.0f, 1.0f) * (shootingSprite.Length - 1));
        spriteRenderer.sprite = shootingSprite[index];
      }
    }
  }
}

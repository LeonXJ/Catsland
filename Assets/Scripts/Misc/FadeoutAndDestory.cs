using UnityEngine;

namespace Catsland.Scripts.Misc {
  public class FadeoutAndDestory :MonoBehaviour {

    public float fadeOutInSeconds = 1.0f;

    private SpriteRenderer[] spriteRenders;
    private float passedSeconds = 0.0f;

    private void Awake() {
      spriteRenders = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update() {
      passedSeconds += Time.deltaTime;
      if(passedSeconds > fadeOutInSeconds) {
        Destroy(gameObject);
      } else {
        float alpha = 1.0f - (passedSeconds / fadeOutInSeconds);
        foreach(SpriteRenderer renderer in spriteRenders) {
          Color color = renderer.material.GetColor("_Color");
          color.a = alpha;
          renderer.material.SetColor("_Color", color);
        }
      }
    }
  }
}

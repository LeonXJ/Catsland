using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Catsland.Scripts.Controller;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Misc {
  public class HealthBar : MonoBehaviour {

    public GameObject heartGO;
    public Vector2 heartInterval = new Vector2(16, 16);
    public int maxHeartsInRow = 10;
    public Sprite emptyHeart;
    public Sprite fullHeart;

    [Header("New Healtbar")]
    public bool useNewHealthBar = false;
    public int widthPerHp = 50;
    public Image back;
    public Image fill;
    public Image delta;
    public float deltaCatchSpeedScale = 2f;

    public float opacity = .1f;

    private List<Image> hearts;

    private void Awake() {
      if (!useNewHealthBar) {
        initializeHearts();
      }
    }

    public void initializeHearts() {
      foreach (Transform child in transform) {
        Destroy(child.gameObject);
      }
      PlayerController playerController = getPlayerController();
      if (playerController == null || heartGO == null) {
        return;
      }

      hearts = new List<Image>();
      for (int i = 0; i < playerController.maxHealth; i++) {
        float currentY = -(i / maxHeartsInRow) * heartInterval.y;
        float currentX = (i % maxHeartsInRow) * heartInterval.x;
        GameObject heart = GameObject.Instantiate(heartGO, transform);
        heart.transform.localPosition = new Vector3(currentX, currentY, 0.0f);
        hearts.Add(heart.GetComponent<Image>());
      }
    }

    void Update() {
      PlayerController playerController = getPlayerController();

      if (useNewHealthBar) {
        // back
        back.rectTransform.sizeDelta = new Vector2(
          playerController.maxHealth * widthPerHp, back.rectTransform.sizeDelta.y);
        // fill
        float width = (float)playerController.currentHealth * back.rectTransform.rect.width
          / playerController.maxHealth;
        fill.rectTransform.sizeDelta = new Vector2(width, fill.rectTransform.sizeDelta.y);
        // delta
        delta.rectTransform.sizeDelta = new Vector2(
          Mathf.Lerp(delta.rectTransform.sizeDelta.x, width, Time.deltaTime * deltaCatchSpeedScale),
          delta.rectTransform.sizeDelta.y);
        return;
      }

      if (playerController.maxHealth != hearts.Count) {
        initializeHearts();
      }

      int index = 0;
      foreach (Image heart in hearts) {
        if (index < playerController.currentHealth
          && heart.sprite != fullHeart) {
          heart.sprite = fullHeart;
        }
        if (playerController.currentHealth <= index
          && heart.sprite != emptyHeart) {
          heart.sprite = emptyHeart;
        }
        heart.color = new Color(heart.color.r, heart.color.g, heart.color.b, opacity);
        index++;
      }
    }

    private PlayerController getPlayerController() {
      return SceneConfig.getSceneConfig().GetPlayer().GetComponent<PlayerController>();
    }
  }


}

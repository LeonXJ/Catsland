using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Catsland.Scripts.Controller;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Misc {
  public class HealthBar: MonoBehaviour {

    public GameObject heartGO;
    public Vector2 heartInterval = new Vector2(16, 16);
    public int maxHeartsInRow = 10;
    public Sprite emptyHeart;
    public Sprite fullHeart;

    public float opacity = .1f;

    private List<Image> hearts;

    private void Awake() {
      initializeHearts();
    }

    public void initializeHearts() {
      foreach(Transform child in transform) {
        Destroy(child.gameObject);
      }
      PlayerController playerController = getPlayerController();
      if(playerController == null || heartGO == null) {
        return;
      }

      hearts = new List<Image>();
      for(int i = 0; i < playerController.maxHealth; i++) {
        float currentY = -(i / maxHeartsInRow) * heartInterval.y;
        float currentX = (i % maxHeartsInRow) * heartInterval.x;
        GameObject heart = GameObject.Instantiate(heartGO, transform);
        heart.transform.localPosition = new Vector3(currentX, currentY, 0.0f);
        hearts.Add(heart.GetComponent<Image>());
      }
    }

    void Update() {
      PlayerController playerController = getPlayerController();
      if(playerController.maxHealth != hearts.Count) {
        initializeHearts();
      }

      int index = 0;
      foreach(Image heart in hearts) {
        if(index < playerController.currentHealth
          && heart.sprite != fullHeart) {
          heart.sprite = fullHeart;
        }
        if(playerController.currentHealth <= index
          && heart.sprite != emptyHeart) {
          heart.sprite = emptyHeart;
        }
        heart.color = new Color(heart.color.r, heart.color.g, heart.color.b, opacity);
        index++;
      }
    }

    private PlayerController getPlayerController() {
      return SceneConfig.getSceneConfig().player.GetComponent<PlayerController>();
    }
  }


}

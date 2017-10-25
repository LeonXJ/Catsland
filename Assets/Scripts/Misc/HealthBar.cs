using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Misc {
  public class HealthBar :MonoBehaviour {

    public PlayerController playerController;
    public GameObject heartGO;
    public Vector2 heartInterval = new Vector2(16, 16);
    public int maxHeartsInRow = 10;
    public Sprite emptyHeart;
    public Sprite fullHeart;

    private List<Image> hearts;

    private void Awake() {
      initializeHearts();
    }

    public void initializeHearts() {
      foreach(Transform child in transform) {
        Destroy(child.gameObject);
      }
      if(playerController == null || heartGO == null) {
        return;
      }

      hearts = new List<Image>();
      for(int i = 0; i < playerController.maxHealth; i++) {
        float currentY = -(i / maxHeartsInRow) * heartInterval.y;
        float currentX = (i % maxHeartsInRow) * heartInterval.x;
        GameObject heart = GameObject.Instantiate(heartGO, transform);
        heart.transform.localPosition =new Vector3(currentX, currentY, 0.0f);
        hearts.Add(heart.GetComponent<Image>());
      }
    }

    void Update() {
      if(playerController.maxHealth != hearts.Count) {
        initializeHearts();
      }

      int index = 0;
      foreach(Image heart in hearts) {
        if(index < playerController.currentHealth
          && hearts[index].sprite != fullHeart) {
          hearts[index].sprite = fullHeart;
        }
        if(playerController.currentHealth <= index
          && hearts[index].sprite != emptyHeart) {
          hearts[index].sprite = emptyHeart;
        }
        index++;
      }
    }
  }


}

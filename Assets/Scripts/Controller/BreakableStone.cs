using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.Controller {
  public class BreakableStone: MonoBehaviour {


    public int maxHealth = 3;
    public int curHealth;

    public Sprite[] sprites;
    public GameObject brokenStoneGoPrefab;

    private SpriteRenderer renderer;

    private void Awake() {
      renderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start() {
      curHealth = maxHealth;
    }

    // Update is called once per frame
    void Update() {
      updateSprite();
    }

    private void updateSprite() {
      int index = (int)(Mathf.Max(curHealth, 0.0f) * (sprites.Length - 1) / maxHealth);
      renderer.sprite = sprites[index];
    }

    public void damage(DamageInfo damageInfo) {
      if(!damageInfo.isSmashAttack) {
        return;
      }
      curHealth -= damageInfo.damage;
      if(curHealth <= 0) {
        GameObject brokenStone = Instantiate(brokenStoneGoPrefab);
        brokenStone.transform.position = transform.position;
        brokenStone.transform.rotation = transform.rotation;
        Destroy(gameObject);
      } else {
        updateSprite();
      }
    }
  }
}

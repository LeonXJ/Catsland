using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Catsland.Scripts.Ui {
  public class OpponentHealthBar : MonoBehaviour {

    public GameObject healthBarOutline;
    public Text opponenetName;
    public Image healthBarBackgournd;
    public Image healthBarFill;

    public bool visible = false;

    private IHealthBarQuery healthBarQuery;

    public void ShowForQuery(IHealthBarQuery healthBarQuery) {
      this.healthBarQuery = healthBarQuery;
      setVisibility(true);
    }

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
      // Warning here: healthBarQuery is not null when the GameObject is destory.
      if (visible && healthBarQuery != null) {
        HealthCondition healthCondition = healthBarQuery.GetHealthCondition();
        if (healthCondition.currentHealth > 0) {
          float width = (float)healthCondition.currentHealth * healthBarBackgournd.rectTransform.rect.width
            / healthCondition.totalHealth;
          healthBarFill.rectTransform.sizeDelta = new Vector2(width, healthBarFill.rectTransform.sizeDelta.y);
          opponenetName.text = healthCondition.name;
          return;
        }
      } 
      if (visible) {
        Debug.Log("Hide opponent health bar.");
        visible = false;
        setVisibility(false);
      }
    }

    private void setVisibility(bool visible) {
      this.visible = visible;
      healthBarOutline.SetActive(visible);
      opponenetName.gameObject.SetActive(visible);
      healthBarBackgournd.gameObject.SetActive(visible);
      healthBarFill.gameObject.SetActive(visible);
    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Catsland.Scripts.Ui {
  public class OpponentHealthBar : MonoBehaviour {

    public Text opponenetName;
    public Image healthBarBackgournd;
    public Image healthBarDelta;
    public Image healthBarFill;

    public float deltaCatchSpeedScale = 2f;
    public bool visible = false;

    private IHealthBarQuery healthBarQuery;
    private CanvasGroup canvasGroup;

    // Whether the bar just went from invisible to visible.
    // If true, the delta will be align with fill. And this bit is reset.
    private bool justEnterVisible = false;

    public void ShowForQuery(IHealthBarQuery healthBarQuery) {
      this.healthBarQuery = healthBarQuery;
      setVisibility(true);
    }

    // Start is called before the first frame update
    void Start() {
      canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update() {
      // Warning here: healthBarQuery is not null when the GameObject is destory.
      if (visible && healthBarQuery != null) {
        HealthCondition healthCondition = healthBarQuery.GetHealthCondition();
        if (healthCondition.currentHealth > 0) {
          // fill
          float width = (float)healthCondition.currentHealth * healthBarBackgournd.rectTransform.rect.width
            / healthCondition.totalHealth;
          healthBarFill.rectTransform.sizeDelta = new Vector2(width, healthBarFill.rectTransform.sizeDelta.y);
          // delta
          healthBarDelta.rectTransform.sizeDelta = justEnterVisible
            ? healthBarFill.rectTransform.sizeDelta
            : new Vector2(
                 Mathf.Lerp(healthBarDelta.rectTransform.sizeDelta.x, width, Time.deltaTime * deltaCatchSpeedScale), 
                 healthBarDelta.rectTransform.sizeDelta.y);
          justEnterVisible = false;
          // text
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
      // enter visible. Align delat bar with fill bar.
      justEnterVisible = !this.visible && visible;
      this.visible = visible;
      canvasGroup.alpha = visible ? 1f : 0f;
    }
  }
}

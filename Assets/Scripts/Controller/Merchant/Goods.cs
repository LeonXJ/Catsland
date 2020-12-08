using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace Catsland.Scripts.Controller.Merchant {

  public class Goods : MonoBehaviour {

    public string trademark = "Durable Ij.";
    public int price = 999;

    public float buyConfirmTime = 1f;
    public TextMeshPro nameText;
    public TextMeshPro priceText;
    public Image priceBackground;
    public Image buyProgress;
    public bool hasDeal = false;

    public GameObject messageReceiver;

    private bool isInTrigger = false;
    private InputMaster inputMaster;
    private float initialBuyTime = -1f;
    private PlayerController playerController;

    private void Awake() {
      inputMaster = new InputMaster();
      inputMaster.General.Interact.performed += _ => {
        if (!Affordable() && isInTrigger) {
          SetPriceBackgroundAlpha(1.0f);
          priceBackground.DOColor(new Color(1f, 1f, 1f, 0f), .5f);
          priceText.transform.DOShakeRotation(.5f, new Vector3(0f, 0f, 20f), 30, 120);

          // message
          messageReceiver?.SendMessage(
            Messages.ON_GOODS_UNAFFORDABLE,
            new Messages.GoodsMessageInfo { trademark = trademark, price = price, goods = this},
            SendMessageOptions.DontRequireReceiver);
        }
      };
    }

    // Start is called before the first frame update
    void Start() {
      SetUiAlpha(0f);
      playerController = GameObject.FindGameObjectWithTag(Common.Tags.PLAYER).GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update() {
      if (isInTrigger && !hasDeal && Affordable()) {
        if (inputMaster.General.Interact.ReadValue<float>() > Mathf.Epsilon) {
          if (initialBuyTime < 0f) {
            initialBuyTime = Time.time;
          }
          // Show progress
          float progress = (Time.time - initialBuyTime) / buyConfirmTime;
          // Deal?
          if (progress > 1f) {
            hasDeal = true;
            Deal();
            // Fall through to hide progress bar
          } else {
            ShowBuyProgress(progress);
            return;
          }
        }
      }
      // Else hide progress bar
      HideBuyProgress();
      initialBuyTime = -1f;
    }

    public void OnSelected() {
      SetTextAlpha(1f);
      SetNameLabel();
      SetPriceLabel();
      isInTrigger = true;

      // Message
      messageReceiver?.SendMessage(
        Messages.ON_GOODS_SELECTED,
        new Messages.GoodsMessageInfo { trademark = trademark, price = price, goods = this },
        SendMessageOptions.DontRequireReceiver);
    }

    public void OnUnselected() {
      SetUiAlpha(0f);
      isInTrigger = false;

      // Message
      messageReceiver?.SendMessage(
        Messages.ON_GOODS_UNSELECTED,
        new Messages.GoodsMessageInfo { trademark = trademark, price = price, goods = this },
        SendMessageOptions.DontRequireReceiver);
    }

    public void UpdatePrice(int price) {
      this.price = price;
      SetNameLabel();
      SetPriceLabel();
    }

    private void Deal() {
      SetUiAlpha(0f);
      // Make effect

      // message
      messageReceiver?.SendMessage(
        Messages.ON_GOODS_PURCHASED,
        new Messages.GoodsMessageInfo { trademark = trademark, price = price },
        SendMessageOptions.DontRequireReceiver);

      // Destroy
      Destroy(gameObject);
    }

    private bool Affordable() => playerController.score >= price;

    private void OnTriggerEnter2D(Collider2D collision) {
      if (!collision.gameObject.CompareTag(Common.Tags.PLAYER)) {
        return;
      }
      collision.gameObject.GetComponent<PlayerGoodsPicker>().RegisterGoods(this);
    }

    private void OnTriggerExit2D(Collider2D collision) {
      if (!collision.gameObject.CompareTag(Common.Tags.PLAYER)) {
        return;
      }
      collision.gameObject.GetComponent<PlayerGoodsPicker>().UnregisterGoods(this);
    }

    private void ShowBuyProgress(float progress) {
      buyProgress.color = Color.white;
      float width = priceBackground.rectTransform.sizeDelta.x * progress;
      buyProgress.rectTransform.sizeDelta = new Vector2(width, buyProgress.rectTransform.sizeDelta.y);
    }

    private void HideBuyProgress() {
      buyProgress.color = new Color(1f, 1f, 1f, 0f);
    }


    private void SetUiAlpha(float alpha) {
      SetTextAlpha(alpha);
      HideBuyProgress();
      SetPriceBackgroundAlpha(alpha);
    }

    private void SetPriceBackgroundAlpha(float alpha) {
      priceBackground.color = new Color(1f, 1f, 1f, alpha);
    }

    private void SetNameLabel() {
      nameText.text = trademark;
      if (Affordable()) {
        nameText.color = Color.white;
      } else {
        nameText.color = Color.red;
      }
    }

    private void SetPriceLabel() {
      priceText.text = "$" + price;
      if (Affordable()) {
        priceText.color = Color.white;
      } else {
        priceText.color = Color.red;
      }
    }

    private void SetTextAlpha(float alpha) {
      nameText.alpha = alpha;
      priceText.alpha = alpha;
    }
    private void OnEnable() {
      inputMaster.Enable();
    }

    private void OnDisable() {
      inputMaster.Disable();
    }
  }
}

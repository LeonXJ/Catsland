using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Catsland.Scripts.Controller.Merchant {
  public class MerchantController : MonoBehaviour {

    [System.Serializable]
    public class MerchantKnowledge {
      public string trademark;
      public string description;
      public int realPrice;
    }

    public string[] idleComments;
    public MerchantKnowledge[] merchantKnowledges;

    public TextMeshPro dialogText;
    public float commentLastForSeconds = 5f;
    public float discountHesitateTime = 2f;

    private float lastCommentTime;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
      if (Time.time - lastCommentTime > commentLastForSeconds) {
        if (idleComments.Length > 0) {
          int index = (int)(idleComments.Length * Random.value);
          Comment(idleComments[index]);
        }
      }
    }

    void OnGoodsSelected(Messages.GoodsMessageInfo info) {
      foreach (var knowledge in merchantKnowledges) {
        if (knowledge.trademark == info.trademark && knowledge.description.Length > 0) {
          Comment(knowledge.description);
        }
      }
    }

    void OnGoodsUnselected(Messages.GoodsMessageInfo info) {
    }

    void OnGoodsUnaffordable(Messages.GoodsMessageInfo info) {
      foreach (var knowledge in merchantKnowledges) {
        if (knowledge.trademark == info.trademark && knowledge.realPrice < info.price && info.goods != null) {
          StartCoroutine(HesitateAndDiscount(info.goods, knowledge.realPrice));
          return;
        }
      }
      Comment("This is the best price you can find in this world.");
    }

    void OnGoodsPurchased(Messages.GoodsMessageInfo info) {
      Comment("Wise choose!");
    }

    private IEnumerator HesitateAndDiscount(Goods goods, int realPrice) {
      yield return new WaitForSeconds(discountHesitateTime);

      goods.UpdatePrice(realPrice);
      Comment("I can give you a discount!");
    }

    private void Comment(string text) {
      dialogText.text = text;
      lastCommentTime = Time.time;
    }
  }
}

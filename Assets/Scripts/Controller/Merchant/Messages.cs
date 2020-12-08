using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller.Merchant {
  public class Messages {

    public const string ON_GOODS_SELECTED = "OnGoodsSelected";
    public const string ON_GOODS_UNSELECTED = "OnGoodsUnselected";
    public const string ON_GOODS_UNAFFORDABLE = "OnGoodsUnaffordable";
    public const string ON_GOODS_PURCHASED = "OnGoodsPurchased";

    public class GoodsMessageInfo {
      public string trademark;
      public int price;

      // Only set when the message is not purchased.
      public Goods goods;
    }
  }
}

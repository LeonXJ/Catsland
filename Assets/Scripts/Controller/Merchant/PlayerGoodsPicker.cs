using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller.Merchant {
  public class PlayerGoodsPicker : MonoBehaviour {

    private HashSet<Goods> goodsSet = new HashSet<Goods>();
    private Goods currentGoods = null;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
      // Clear up destroyed goods
      goodsSet.RemoveWhere(goods => goods == null);

      // Find the nearest goods.
      float minDistanceSqr = float.MaxValue;
      Goods nearestGoods = null;
      foreach (var goods in goodsSet) {
        Vector2 delta = transform.position - goods.gameObject.transform.position;
        float distanceSqr = delta.sqrMagnitude;
        if (distanceSqr < minDistanceSqr) {
          minDistanceSqr = distanceSqr;
          nearestGoods = goods;
        }
      }
      if (nearestGoods != currentGoods) {
        if (currentGoods != null && !currentGoods.hasDeal) {
          // On unselected
          currentGoods.OnUnselected();
        }
        if (nearestGoods != null) {
          // On selected
          nearestGoods.OnSelected();
        }
        currentGoods = nearestGoods;
      }
    }

    public void RegisterGoods(Goods goods) {
      if (!goodsSet.Contains(goods)) {
        goodsSet.Add(goods);
      }
    }

    public void UnregisterGoods(Goods goods) {
      if (goodsSet.Contains(goods)) {
        goodsSet.Remove(goods);
      }
    }

  }
}

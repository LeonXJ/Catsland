using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.Controller {
  public class Scarecrow : MonoBehaviour {

    public Timing timing;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void OnDamageByPass(DamageBypassInfo damageBypassInfo) {
      damageBypassInfo.damageInfo.onDamageFeedback?.Invoke(new DamageInfo.DamageFeedback(true));
      transform.DOShakePosition(timing.EnemyHitShakeTime, new Vector3(0.1f, 0), 30, 120);
    }
  }
}

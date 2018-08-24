﻿using UnityEngine;
using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.Misc {
  public class DamageSplash: MonoBehaviour {

    public GameObject splashPrefab;

    public void damage(DamageInfo damageInfo) {
      GameObject splash = Instantiate(splashPrefab);
      splash.transform.position = damageInfo.damagePosition;
      splash.transform.localScale =
        new Vector3(Mathf.Sign(damageInfo.repelDirection.x), 1.0f, 1.0f);
    }
  }
}
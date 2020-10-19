using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Bullets {
  public class DamageBypassInfo {

    public DamageInfo damageInfo;
    public string byPasser;

    public DamageBypassInfo(DamageInfo damageInfo, string byPasser) {
      this.damageInfo = damageInfo;
      this.byPasser = byPasser;
    }
  }
}

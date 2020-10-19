using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Bullets {

  public class DamageBypass : MonoBehaviour {
    public GameObject damageAcceptor;

    public void damage(DamageInfo damageInfo) {
      damageAcceptor.SendMessage(
        MessageNames.DAMAGE_BYPASS_FUNCTION,
        new DamageBypassInfo(damageInfo, gameObject.name),
        SendMessageOptions.DontRequireReceiver);
    }
  }
}

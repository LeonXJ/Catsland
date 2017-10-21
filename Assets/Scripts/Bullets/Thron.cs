using UnityEngine;

using Catsland.Scripts.Common;

namespace Catsland.Scripts.Bullets {
  public class Thron :MonoBehaviour {

    public int damage = 1;
    public float repelIntense = 1.0f;

    public LayerMask blacklistLayer;

    public void OnTriggerEnter2D(Collider2D collision) {
      // if not blacklisted
      if((collision.gameObject.layer & blacklistLayer) == 0) {
        Vector2 repelDirection =
          collision.gameObject.transform.position - transform.position;
        collision.gameObject.SendMessage(
          MessageNames.DAMAGE_FUNCTION,
          new DamageInfo(damage, repelDirection, repelIntense),
          SendMessageOptions.DontRequireReceiver);
      }


    }

  }



}

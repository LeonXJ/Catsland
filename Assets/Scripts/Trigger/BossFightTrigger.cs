using UnityEngine;
using Catsland.Scripts.Misc;

namespace Catsland.Scripts.Trigger {
  public class BossFightTrigger : MonoBehaviour {

    public ArenaDirector arenaDirector;
    public bool oneTime = true;

    private bool hasTriggerred = false;

    private void OnTriggerEnter2D(Collider2D collision) {
      if (hasTriggerred) {
        return;
      }
      if (collision.gameObject.tag != Common.Tags.PLAYER) {
        return;
      }
      arenaDirector.PlayFromBeginning();
      hasTriggerred = true;
    }
  }
}

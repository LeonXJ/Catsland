using UnityEngine;

namespace Catsland.Scripts.Misc {

  [RequireComponent(typeof(Collider2D))]
  public class Savepoint : MonoBehaviour {

    public string portalName;

    private void OnTriggerEnter2D(Collider2D collision) {
      if (collision.gameObject.CompareTag(Common.Tags.PLAYER)) {
        SceneMaster.getInstance().Save(portalName);
      }
    }
  }
}

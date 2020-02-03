using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Bullets {
  public class ContactDamage : MonoBehaviour {

    public delegate void OnHitEvent();

    public int damage;
    public float repelIntensity;
    public GameObject owner;
    public bool isSmashAttack = false;
    public bool canSmashAttakInStay = false;
    public bool tryGetPartyTagFromParent = true;

    public bool presetRepelDirection = false;
    public Vector2 repelDirection;

    public OnHitEvent onHitEvent;

    public LayerMask includeLayer;

    private PartyTag partyTag;

    private void Start() {
      partyTag = transform.parent.GetComponent<PartyTag>();
    }


    void OnCollisionEnter2D(Collision2D collision) {
      onHit(collision, false);
    }

    void OnTriggerEnter2D(Collider2D collider) {
      onHit(collider, false);
    }

    void OnCollisionStay2D(Collision2D collision) {
      onHit(collision, true);
    }

    private void OnTriggerStay2D(Collider2D collision) {
      onHit(collision, true);
    }

    void onHit(Collision2D collision, bool isStay) {
      onHitGameObject(collision.collider, isStay);
    }

    void onHit(Collider2D collider, bool isStay) {
      onHitGameObject(collider, isStay);
    }

    private void onHitGameObject(Collider2D collider, bool isStay) {
      if (!enabled) {
        return;
      }
      GameObject collidingGameObject = collider.gameObject;

      bool masked = (includeLayer & (1 << collider.gameObject.layer)) == 0x0;
      PartyTag otherPartyTag = collidingGameObject.GetComponent<PartyTag>();
      if (!masked && collidingGameObject != owner && PartyTag.ShouldTakeDamage(partyTag, otherPartyTag)) {
        IMeleeDamageInterceptor interceptor = collidingGameObject.GetComponent<IMeleeDamageInterceptor>();
        if (interceptor == null || interceptor.getMeleeResult().status == MeleeResultStatus.HIT) {
          Vector2 delta = collidingGameObject.transform.position - transform.position;
          collidingGameObject.SendMessage(
            MessageNames.DAMAGE_FUNCTION,
            new DamageInfo(
              damage, collider.bounds.center, presetRepelDirection ? repelDirection : new Vector2(Mathf.Sign(delta.x), 0.0f), repelIntensity,
              isSmashAttack && (canSmashAttakInStay || !isStay)),
            SendMessageOptions.DontRequireReceiver);
          onHitEvent?.Invoke();
        }
      }
    }
  }
}

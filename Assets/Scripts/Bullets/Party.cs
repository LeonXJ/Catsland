using System.IO.Ports;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Catsland.Scripts.Bullets {
  public class Party : MonoBehaviour {

    public enum PartyTag {
      ALL = 1,
      PLAYER = 2,
      ENEMY = 3,
    }

    public PartyTag[] parties;

    public bool IsMebmerOf(PartyTag partyTag) {
      if (partyTag == PartyTag.ALL) {
        return true;
      }
      return parties.Contains(partyTag);
    }


    [System.Serializable]
    public class WeaponPartyConfig {
      public PartyTag[] hitPartyTags;
      public PartyTag[] ignorePartyTags;
    
      public bool shouldHitParty(Party party) {
        // In hit and not in ignore
        if (party == null) {
          return hitPartyTags.Contains(PartyTag.ALL) && !ignorePartyTags.Contains(PartyTag.ALL);
        }

        bool isInHit = hitPartyTags.Any(tag => party.IsMebmerOf(tag));
        if (!isInHit) {
          return false;
        }
        return !ignorePartyTags.Any(tag => party.IsMebmerOf(tag));
      }
    }
  }
}

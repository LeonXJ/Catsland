using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Misc {
  public class Ground : MonoBehaviour {

    public Sound.Sound footstepSound;

    public Sound.Sound getFootstepSound() {
      return footstepSound;
    }
  }
}

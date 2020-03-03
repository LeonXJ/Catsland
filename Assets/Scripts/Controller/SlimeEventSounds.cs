using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Sound;

namespace Catsland.Scripts.Controller {

  public class SlimeEventSounds : CharacterEventSounds {

    public Sound.Sound jumpOffSound;
    public Sound.Sound jumpLandSound;

    public void PlayJumpOffSound() {
      jumpOffSound?.Play(eventAudioSource);
    }

    public void PlayLandSound() {
      jumpLandSound?.Play(eventAudioSource);
    }
  }
}

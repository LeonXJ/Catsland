using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class BanditEventSounds : CharacterEventSounds {

    public AudioSource continousAudioSource;
    public Sound.Sound chargePrepareSound;
    public Sound.Sound chargeSound;
    public Sound.Sound spellSound;
    public Sound.Sound jumpPrepare;
    public Sound.Sound jumpOff;
    public Sound.Sound jumpLand;
    public Sound.Sound prepareUnleash;

    public void PlayChargePrepareSound() {
      chargePrepareSound?.Play(eventAudioSource);
    }

    public void PlayChargeSound() {
      chargeSound?.Play(eventAudioSource);
    }

    public void PlaySpellSound() {
      spellSound?.Play(eventAudioSource);
    }

    public void PlayJumpPrepare() {
      jumpPrepare?.Play(eventAudioSource);
    }

    public void PlayJumpOff() {
      jumpOff?.Play(eventAudioSource);
    }

    public void PlayJumpLand() {
      jumpLand?.Play(eventAudioSource);
    }

    public void StopContinousSound() {
      continousAudioSource.Stop();
    }

    public void PlayPrepareUnleash() {
      prepareUnleash?.Play(eventAudioSource);
    }
  }
}

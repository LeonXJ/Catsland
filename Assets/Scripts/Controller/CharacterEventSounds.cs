using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class CharacterEventSounds : MonoBehaviour {

    public AudioSource eventAudioSource;
    public Sound.Sound damageSound;
    public Sound.Sound dieSound;

    public void PlayOnDamageSound() {
      damageSound?.Play(eventAudioSource);
    }

    public void PlayOnDieSound() {
      dieSound?.PlayOneShot(transform.position);
    }
  }
}

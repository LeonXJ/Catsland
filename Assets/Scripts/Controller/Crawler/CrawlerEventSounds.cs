using UnityEngine;
namespace Catsland.Scripts.Controller.Crawler {
  public class CrawlerEventSounds : CharacterEventSounds {

    public AudioSource walkingSoundAudioSource;
    public Sound.Sound walkingSound;

    public void ContinuePlayWalkingSound() {
      walkingSound.PlayIfNotPlaying(walkingSoundAudioSource);
    }

    public void PauseWalkingSound() {
      walkingSoundAudioSource.Pause();
    }
  }
}

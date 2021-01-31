namespace Catsland.Scripts.Controller.Archer {
  public class ArcherEventSounds : CharacterEventSounds {

    public Sound.Sound releaseSound;

    public void PlayReleaseSound() {
      releaseSound?.Play(eventAudioSource);
    }
  }
}

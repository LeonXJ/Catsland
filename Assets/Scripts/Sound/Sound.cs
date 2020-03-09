using UnityEngine;

namespace Catsland.Scripts.Sound {
  [CreateAssetMenu(menuName = "ScriptableObjects/Sound", order = 1)]
  public class Sound : ScriptableObject {

    public AudioClip[] audioClips;

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0f, .5f)]
    public float randomVolume = .1f;

    [Range(.5f, 1.5f)]
    public float pitch = 1f;
    [Range(0f, .5f)]
    public float randomPitch = .1f;

    [System.Serializable]
    public class SoundSetting {
      [Range(0f, 1f)]
      public float volume = 1f;
      [Range(0f, .5f)]
      public float randomVolume = .1f;

      [Range(.5f, 1.5f)]
      public float pitch = 1f;
      [Range(0f, .5f)]
      public float randomPitch = .1f;

      public SoundSetting(float volume, float randomVolume = 0f, float pitch = 1f, float randomPitch = 0f) {
        this.volume = volume;
        this.randomVolume = randomVolume;
        this.pitch = pitch;
        this.randomPitch = randomPitch;
      }
    }

    public float extraLifetimeForOneshot = .1f;

    // Play on the given AudioSource, returns the length of audio clip.
    public float Play(AudioSource audioSource, SoundSetting soundSetting = null) {
      if (audioClips.Length == 0) {
        Debug.LogError("No audio clip is set for " + name);
        return 0f;
      }
      AudioClip audioClip = GetAudioClip();
      audioSource.clip = audioClip;

      SoundSetting setting = soundSetting;
      if (setting == null) {
        setting = new SoundSetting(volume, randomVolume, pitch, randomPitch);
      }

      audioSource.volume = setting.volume * (1 + Random.Range(-setting.randomVolume * .5f, setting.randomVolume* .5f));
      audioSource.pitch = setting.pitch * (1 + Random.Range(-setting.randomPitch * .5f, setting.randomPitch * .5f));
      audioSource.Play();
      return audioClip.length;
    }

    public AudioClip GetAudioClip() {
      return audioClips[Random.Range(0, audioClips.Length)];
    }

    public void PlayOneShot(Vector3 position) {
      GameObject gameObject = new GameObject("Oneshot of " + name);
      gameObject.transform.position = position;
      AudioSource audioSource = gameObject.AddComponent<AudioSource>();
      float length = Play(audioSource);
      Destroy(gameObject, length + extraLifetimeForOneshot);
    }

    public void PlayIfNotPlaying(AudioSource audioSource) {
      // TODO: check whether the clip is within the clip list.
      bool isCurrentClipDesired = false;
      foreach(AudioClip clip in audioClips) {
        if (clip == audioSource.clip) {
          isCurrentClipDesired = true;
          break;
        }
      }

      if (!audioSource.isPlaying || !isCurrentClipDesired) {
        Play(audioSource);
      }
    }
  }
}

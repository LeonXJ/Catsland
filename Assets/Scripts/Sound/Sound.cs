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

    public float extraLifetimeForOneshot = .1f;

    // Play on the given AudioSource, returns the length of audio clip.
    public float Play(AudioSource audioSource) {
      if (audioClips.Length == 0) {
        Debug.LogError("No audio clip is set for " + name);
        return 0f;
      }
      AudioClip audioClip = audioClips[Random.Range(0, audioClips.Length)];
      audioSource.clip = audioClip;
      audioSource.volume = volume * (1 + Random.Range(-randomVolume * .5f, randomVolume * .5f));
      audioSource.pitch = pitch * (1 + Random.Range(-randomPitch * .5f, randomPitch * .5f));
      audioSource.Play();
      return audioClip.length;
    }

    public void PlayOneShot(Vector3 position) {
      GameObject gameObject = new GameObject("Oneshot of " + name);
      gameObject.transform.position = position;
      AudioSource audioSource = gameObject.AddComponent<AudioSource>();
      float length = Play(audioSource);
      Destroy(gameObject, length + extraLifetimeForOneshot);
    }
  }
}

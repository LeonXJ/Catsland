using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catsland.Scripts.Sound {


  public class MusicManager : MonoBehaviour {

    private static MusicManager instance;

    [System.Serializable]
    public class MusicTrack {
      public AudioSource audioSource;
      public float offsetInS = 0f;
      public float targetVolume = 1.0f;
      public float transitionSpeedPerS = .1f;
      public float currentVolume;
      private float currentOffsetInS = 0f;

      public void Stop() {
        audioSource.Stop();
      }

      public void Play(Sound sound, bool loop) {
        audioSource.loop = loop;
        sound.Play(audioSource);
      }

      public void SetVolume(float volume) {
        audioSource.volume = volume;
        targetVolume = volume;
      }

      public float getVolume() {
        return audioSource.volume;
      }

      public bool IsPlaying() {
        return audioSource.isPlaying;
      }

      public void StartTransition(float targetVolume, float transitionInS, float offsetInS = 0f) {
        this.targetVolume = targetVolume;
        this.transitionSpeedPerS = transitionInS == 0 ? Mathf.Infinity : 1f / transitionInS;
        this.offsetInS = offsetInS;
        this.currentOffsetInS = 0f;
      }

      public void Update(float deltaTime) {
        float delta = targetVolume - audioSource.volume;
        if (delta * delta > Mathf.Epsilon) {
          currentOffsetInS += deltaTime;
          if (currentOffsetInS > offsetInS) {
            if (delta > 0) {
              audioSource.volume = Mathf.Min(targetVolume, audioSource.volume + transitionSpeedPerS * deltaTime);
            } else {
              audioSource.volume = Mathf.Max(targetVolume, audioSource.volume - transitionSpeedPerS * deltaTime);
            }
          }
        }
        if (targetVolume < Mathf.Epsilon && audioSource.volume < Mathf.Epsilon) {
          audioSource.Stop();
        }
        currentVolume = audioSource.volume;
      }
    }

    public MusicTrack[] musicTracks = new MusicTrack[2];
    private int mainSource = 0;

    public static MusicManager getInstance() {
      return instance;
    }

    void Awake() {
      // Make sure only has one instance event during scene change.
      if (instance != null) {
        if (instance != this) {
          Destroy(this);
        }
      } else {
        instance = this;
      }
    }

    public void PlayImmediately(Sound sound, bool loop = true) {
      mainSource = 0;
      foreach (MusicTrack track in musicTracks) {
        track.Stop();
      }
      musicTracks[mainSource].Play(sound, loop);
    }

    public void PlayWithTransition(
        Sound sound, float outTransitionInS,
        float inOffsetInS, float inTransitionInS, bool loop = true) {
      MusicTrack primary = musicTracks[mainSource];

      if (!primary.IsPlaying()) {
        PlayImmediately(sound, loop);
        primary.Play(sound, loop);
        primary.SetVolume(0f);
        primary.StartTransition(1f, inTransitionInS, inOffsetInS);
        return;
      }

      MusicTrack secondary = mainSource == 0 ? musicTracks[1] : musicTracks[0];
      if (!secondary.IsPlaying()) {
        // Ramp down primary and ramp up secondary
        mainSource = 1;
        primary.StartTransition(0f, outTransitionInS);
        secondary.Play(sound, loop);
        secondary.SetVolume(0f);
        secondary.StartTransition(1f, inTransitionInS, inOffsetInS);
      }
    }

    public void Stop(float transitionTimeS) {
      Debug.Log("MusicManager is stopping all playings.");
      MusicTrack primary = musicTracks[mainSource];
      if (!primary.IsPlaying()) {
        Debug.Log("Didn't stop anything because nothing is playing.");
        return;
      }
      primary.StartTransition(0f, transitionTimeS);
    }

    void FixedUpdate() {
      foreach (MusicTrack track in musicTracks) {
        track.Update(Time.deltaTime);
      }
    }
  }
}

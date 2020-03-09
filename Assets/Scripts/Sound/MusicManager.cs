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

      public void StartTransition(float targetVolume, float transitionInS, float offsetInS = 0f, bool resetVolume = true) {
        this.targetVolume = targetVolume;
        this.transitionSpeedPerS = transitionInS == 0 ? Mathf.Infinity : 1f / transitionInS;
        this.offsetInS = offsetInS;
        this.currentOffsetInS = 0f;
        if (resetVolume) {
          this.audioSource.volume = 0f;
        }
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
            // if not playing, start playing
            if (!audioSource.isPlaying) {
              audioSource.Play();
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

    [Header("Debug")]
    public Sound[] debugSounds;
    public int debugSoundIndex = -1;
    public float debugOutTransitionInS = 1f;
    public float debugInTransitionInS = 2f;
    public float debugInOffsetInS = 2f;

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
        primary.audioSource.clip = sound.GetAudioClip();
        primary.StartTransition(1f, inTransitionInS, inOffsetInS);
        return;
      }

      MusicTrack secondary = mainSource == 0 ? musicTracks[1] : musicTracks[0];
      if (!secondary.IsPlaying()) {
        // Ramp down primary and ramp up secondary
        // Switch main source.
        mainSource = 1 - mainSource;
        primary.StartTransition(0f, outTransitionInS, /*offsetInS=*/ 0f, /*resetVolume=*/ false);
        secondary.audioSource.clip = sound.GetAudioClip();
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
      primary.StartTransition(0f, transitionTimeS, /*offsetInS=*/ 0f, /*resetVolume=*/ false);
    }

    void Update() {
      debugUpdate();
    }

    void FixedUpdate() {
      foreach (MusicTrack track in musicTracks) {
        track.Update(Time.deltaTime);
      }
    }
    private void debugUpdate() {
      if (Input.GetKeyDown(KeyCode.Alpha1)) {
        if (debugSounds.Length == 0) {
          Debug.Log("Debug MusicManager> error: no debug sound is set.");
          return;
        }

        debugSoundIndex = (debugSoundIndex + 1) % debugSounds.Length;
        Debug.Log("Debug MusicManager> Play next index: " + debugSoundIndex);
        PlayWithTransition(debugSounds[debugSoundIndex], debugOutTransitionInS, debugInOffsetInS, debugInTransitionInS, true);
      }
      if (Input.GetKeyDown(KeyCode.Alpha2)) {
        Stop(debugOutTransitionInS);
      }
    }
  }
}

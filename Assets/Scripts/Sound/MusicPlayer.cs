using UnityEngine;

namespace Catsland.Scripts.Sound {
  public class MusicPlayer : MonoBehaviour {

    public Sound sound;
    public bool loop = true;
    public float outTransitionInS = 0f;
    public float inTransitionInS = 0f;
    public float inOffsetInS = 0f;
    public void Play(float transition = 0f) {
      MusicManager.getInstance().PlayWithTransition(sound, transition, /* inOffsetInS= */ 0f, transition ,loop);
    }

    public void Play() {
      MusicManager.getInstance().PlayWithTransition(
        sound, outTransitionInS, inOffsetInS, inTransitionInS, loop);
    }

    public void Stop(float transition) {
      MusicManager.getInstance().Stop(transition);
    }
  }
}

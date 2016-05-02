using UnityEngine;
using System.Collections;

namespace Catslandx {
  public class SoundPackageInformation {
    private GameObject soundMaker;
    private Vector3 position;

    public SoundPackageInformation(Vector3 position, GameObject soundMaker) {
      this.position = position;
      this.soundMaker = soundMaker;
    }

    public GameObject getSoundMaker() {
      return soundMaker;
    }

    public Vector3 getPosition() {
      return position;
    }

  }
}

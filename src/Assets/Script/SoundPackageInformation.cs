using UnityEngine;
using System.Collections;

namespace Catslandx {
  public class SoundPackageInformation {
    private GameObject soundMaker;

    public SoundPackageInformation(GameObject soundMaker) {
      this.soundMaker = soundMaker;
    }

    public GameObject getSoundMaker() {
      return soundMaker;
    }

  }
}

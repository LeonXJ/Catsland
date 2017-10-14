using UnityEngine;
using System.Collections;

namespace Catslandx {
  public interface ISoundReceiver {
    void receive(float volume, SoundPackageInformation soundPackageInformation);
  }
}

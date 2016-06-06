using UnityEngine;
using System.Collections;

namespace Catslandx {
  public interface IObjectDisposalReceiver {
    bool selfDestroy();
  }
}

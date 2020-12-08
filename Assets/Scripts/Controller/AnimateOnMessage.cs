using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class AnimateOnMessage : MonoBehaviour {

    private const string PARAM_START = "Start";

    public bool StartOnDestroy = false;

    private Animator animator;

    private void Start() {
      animator = GetComponent<Animator>();
    }

    // Triggerred by message.
    private void OnDestroyed(OnDestroyedInfo onDestroyedInfo) {
      if (!StartOnDestroy) {
        return;
      }
      animator.SetTrigger(PARAM_START);
    }

  }
}

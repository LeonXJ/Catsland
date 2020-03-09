using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class MenuController : MonoBehaviour {

    private const string SHOW_MENU = "ShowMenu";

    public PlayableDirector exitMenuDirector;
    public Sound.Sound uiConfirm;
    public bool showMenu = true;
    public float musicFadeOutInS = 5f;

    private Animator menuAnimator;

    // Start is called before the first frame update
    void Start() {
      menuAnimator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update() {
      if (showMenu && Input.GetButtonDown("Attack")) {
        showMenu = false;
        exitMenuDirector.Play();
        uiConfirm?.Play(SceneConfig.getSceneConfig().GetUiAudioSource());
        SceneConfig.getSceneConfig().GetMusicManager().Stop(musicFadeOutInS);
      }
    }

    public void HideMenu() {
      menuAnimator.SetBool(SHOW_MENU, false);
    }
  }
}

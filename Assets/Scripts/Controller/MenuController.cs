using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Catsland.Scripts.Controller {
  public class MenuController : MonoBehaviour {

    private const string SHOW_MENU = "ShowMenu";

    public PlayableDirector exitMenuDirector;

    private Animator menuAnimator;

    // Start is called before the first frame update
    void Start() {
      menuAnimator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update() {
      if (Input.GetButtonDown("Attack")) {
        exitMenuDirector.Play();
      }
    }

    public void HideMenu() {
      menuAnimator.SetBool(SHOW_MENU, false);
    }
  }
}

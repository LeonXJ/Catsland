using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;
using Catsland.Scripts.Misc;
using Catsland.Scripts.Sound;

namespace Catsland.Scripts.Ui {
  public class MainMenu : MonoBehaviour {

    private const string DO_NOT_DESTROY_SCENE_NAME = "NotDestroy";

    public SceneMaster.SceneTransitInfo firstSceneInfo;
    public PlayerController.Snapshot initialPlayerStatus;

    private MusicPlayer musicPlayer;
    private GameObject gameMaster;

    // Start is called before the first frame update
    void Start() {
      musicPlayer = GetComponent<MusicPlayer>();
      gameMaster = GameObject.FindGameObjectWithTag(Tags.GAME_MASTER);
      if (gameMaster == null) {
        SceneManager.LoadSceneAsync(DO_NOT_DESTROY_SCENE_NAME, LoadSceneMode.Additive).completed
          += GameMasterLoaded;
      } else {
        PlayMenuMusic();
      }
    }

    public void OnStart () {
      SceneMaster sceneMaster = SceneMaster.getInstance();
      sceneMaster.Save(firstSceneInfo, initialPlayerStatus);
      sceneMaster.TransitionToScene(firstSceneInfo, initialPlayerStatus);
    }

    public void OnQuit () {
      Debug.Log("Quit.");
      Application.Quit();
    }


    private void GameMasterLoaded(AsyncOperation obj) {
      gameMaster = GameObject.FindGameObjectWithTag(Tags.GAME_MASTER);
      if (gameMaster == null) {
        Debug.LogErrorFormat("Unable to find GameMaster game object with tag: {0}.", DO_NOT_DESTROY_SCENE_NAME);
        return;
      }
      PlayMenuMusic();
    }

    private void PlayMenuMusic() {
      Debug.Assert(musicPlayer != null, "No MusicPlayer attaches to MainMenu.");
      musicPlayer.Play();
    }
  }
}

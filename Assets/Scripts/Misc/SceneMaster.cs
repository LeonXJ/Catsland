using Catsland.Scripts.Controller;
using Panda.Examples.PlayTag;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Catsland.Scripts.Misc {
  public class SceneMaster : MonoBehaviour {

    public Animator animator;

    private static SceneMaster instance;
    private InputMaster inputMaster;

    private PlayerController.Snapshot playerSnapshot;
    private SceneTransitInfo transitTarget;

    // Save and load
    private PlayerController.Snapshot savedPlayerSnapshot;
    private SceneTransitInfo savedTransitTarget;

    private readonly string PARAM_START = "Start";
    private readonly string PARAM_DONE = "Done";

    [System.Serializable]
    public class SceneTransitInfo {

      public SceneTransitInfo(string sceneName, string portalName) {
        this.sceneName = sceneName;
        this.portalName = portalName;
      }

      public string sceneName;
      public string portalName;
    }

    public static SceneMaster getInstance() {
      return instance;
    }

    void Awake() {
      instance = this;
      DontDestroyOnLoad(gameObject);
      inputMaster = new InputMaster();
      inputMaster.Debug.LoadScene1.performed += _ => {
        Debug.Log("Transite to Forest 2");
        TransitionToScene(new SceneTransitInfo("Forest2", "From Forest1"));
      };
    }

    private void OnEnable() {
      inputMaster.Enable();
    }

    private void OnDisable() {
      inputMaster.Disable();
    }

    // If init player snapshot is not set, then carry over from current scene.
    public void TransitionToScene(SceneTransitInfo transitTarget, PlayerController.Snapshot initPlayerSnapshot = null) {
      playerSnapshot = initPlayerSnapshot ?? generatePlayerSnapshot();
      this.transitTarget = transitTarget;
      // StartFinish() will be called when the animiation is done.
      animator.SetTrigger(PARAM_START);
    }

    public void Save(string activeScenePortalName) {
      Save(
        new SceneTransitInfo(SceneManager.GetActiveScene().name, activeScenePortalName),
        generatePlayerSnapshot());
    }

    public void Save(SceneTransitInfo sceneTransitInfo, PlayerController.Snapshot playerSnapshot) {
      savedPlayerSnapshot = playerSnapshot;
      savedTransitTarget = sceneTransitInfo;
      Debug.Log("Game saved on portal: " + sceneTransitInfo.sceneName + " with player snapshot: " + playerSnapshot);
    }

    public void LoadLatest() {
      Debug.Log("Load latest. Scene " + savedTransitTarget.sceneName + " portal: " + savedTransitTarget.portalName);
      playerSnapshot = savedPlayerSnapshot;
      transitTarget = savedTransitTarget;
      animator.SetTrigger(PARAM_START);
    }

    // Called when the start transition animation is done.
    public void StartFinish() {
      SceneManager.LoadSceneAsync(transitTarget.sceneName, LoadSceneMode.Single).completed 
        += SceneMaster_completed;
    }

    private void SceneMaster_completed(AsyncOperation obj) {
      SceneInitializer initializer = FindObjectOfType<SceneInitializer>();
      Debug.Assert(initializer != null, "The scene " + SceneManager.GetActiveScene().name + " doesn't have initializer.");
      initializer?.initializeScene(transitTarget.portalName);
      syncToPlayerSnapshot(playerSnapshot);

      animator.SetTrigger(PARAM_DONE);
    }

    private PlayerController.Snapshot generatePlayerSnapshot() {
       return GameObject.FindGameObjectWithTag(Common.Tags.PLAYER).GetComponent<PlayerController>().generateSnapshot();
    }

    private void syncToPlayerSnapshot(PlayerController.Snapshot snapshot) {
      GameObject.FindGameObjectWithTag(Common.Tags.PLAYER).GetComponent<PlayerController>().syncToSnapshot(snapshot);
    }
  }
}

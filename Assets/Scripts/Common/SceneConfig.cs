﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using Catsland.Scripts.Camera;
using Catsland.Scripts.Misc;
using Catsland.Scripts.Sound;
using Catsland.Scripts.Ui;

namespace Catsland.Scripts.Common {
  public class SceneConfig: MonoBehaviour {

    private static SceneConfig sceneConfig;
    private const string DO_NOT_DESTROY_SCENE_NAME = "NotDestroy";

    public UnityEngine.Camera MainCamera;
    public GlobalLightController globalLightController;
    public ProgressManager progressManager;
    public Shader defaultDiffuseShader;
    public FocusPointController cameraController;
    public CinemachineImpulseSource stonePillarShakeAgent;
    public GameObject enemyTitle;
    public Animator enemyTitleBlack;
    public UnityEngine.Camera lightCamera;
    public GameObject player;
    public ArenaDirector arenaDirector;
    public RippleEffect rippleEffect;

    private OpponentHealthBar opponentHealthBar;

    [Header("Sound")]
    public AudioSource uiAudioSource;

    [Header("Debug")]
    public bool useDebugInitialPosition = true;
    public Transform playerInitialPosition;

    private GameObject gameMaster;

    public static SceneConfig getSceneConfig() {
      return sceneConfig;
    }

    SceneConfig() {
      sceneConfig = this;
      progressManager = new ProgressManager();
    }

    public void Awake() {
      gameMaster = GameObject.FindGameObjectWithTag(Tags.GAME_MASTER);
      if (gameMaster == null) {
        SceneManager.LoadSceneAsync(DO_NOT_DESTROY_SCENE_NAME, LoadSceneMode.Additive);
      }
    }

    public void Start() {
      if (playerInitialPosition != null && useDebugInitialPosition) {
        player.transform.position = playerInitialPosition.transform.position;
      }
      opponentHealthBar = FindObjectOfType<OpponentHealthBar>();
    }

    public GameObject GetPlayer() {
      return GameObject.FindGameObjectWithTag(Tags.PLAYER);
    }

    public ProgressManager getProgressManager() {
      return progressManager;
    }

    public FocusPointController GetCameraController() {
      return cameraController;
    }

    public AudioSource GetUiAudioSource() {
      return uiAudioSource;
    }

    public MusicManager GetMusicManager() {
      return MusicManager.getInstance();
    }

    public OpponentHealthBar GetOpponentHealthBar() {
      return opponentHealthBar;
    }

    public Shader GetDefaultDiffuseShader() {
      return defaultDiffuseShader;
    }

    public void DisplayEnemyTitle(string text) {
      if (enemyTitle == null) {
        return;
      }
      Text title = enemyTitle.GetComponent<Text>();
      title.text = text;
      Animator ani = enemyTitle.GetComponent<Animator>();
      ani.SetTrigger("ShowTitle");

      if (enemyTitleBlack != null) {
        enemyTitleBlack.SetTrigger("");
      }
    }

    public void stonePillarShake(Vector3 position, Vector3 velocity) {
      stonePillarShakeAgent.GenerateImpulseAt(position, velocity);
    }

  }
}

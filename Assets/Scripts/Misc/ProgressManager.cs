using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Misc {
  public class ProgressManager {

    private Progress progress;

    public void Save(Progress progress) {
      this.progress = progress;
    }

    public void Load() {
      if (progress == null) {
        reloadScene();
        return;
      }

      GameObject player = SceneConfig.getSceneConfig().GetPlayer();
      Debug.Assert(player != null, "Cannot find player GameOject.");

      PlayerController playerController = player.GetComponent<PlayerController>();
      Debug.Assert(playerController != null, "Play object should contain a PlayerController component.");

      player.transform.position = progress.getPosition();
      playerController.currentHealth = progress.getHp();
      playerController.score = progress.getScore();
    }

    private void reloadScene() {
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public class Progress {
      private int hp;
      private Vector3 position;
      private int score;

      public static Progress Create(GameObject playerObject) {
        Progress progress = new Progress();

        PlayerController playerController = playerObject.GetComponent<PlayerController>();
        Debug.Assert(playerController != null, "Play object should contain a PlayerController component.");

        progress.hp = playerController.currentHealth;
        progress.position = playerController.transform.position;
        progress.score = playerController.score;

        return progress;
      }

      public Vector3 getPosition() {
        return position;
      }

      public int getHp() {
        return hp;
      }

      public int getScore() {
        return score;
      }
    }

  }
}


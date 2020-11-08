using Catsland.Scripts.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.SceneInitializer;

namespace Catsland.Scripts.Misc {
  public class SceneInitializer : MonoBehaviour {

    [System.Serializable]
    public class PortalInfo {
      public string portalName;
      public Transform portalPosition;
    }

    public List<PortalInfo> portals;
    public List<SpecificSceneInitializerBase> specificSceneInitializer;

    private Transform checkpoint;

    // Optionally set the save point to current portal. Use for loading from main menu.
    public void initializeScene(string portalName) {
      if (specificSceneInitializer != null) {
        specificSceneInitializer.ForEach((initializer) => initializer.process());
      }

      bool foundPortal = false;
      // portal
      foreach (PortalInfo portal in portals) {
        if (portal.portalName == portalName) {
          setPlayerToPosition(portal.portalPosition.position);
          checkpoint = portal.portalPosition;
          foundPortal = true;
          break;
        }
      }
      // save point
      if (!foundPortal) {
        Savepoint[] savepoints = FindObjectsOfType<Savepoint>();
        foreach (Savepoint savepoint in savepoints) {
          if (savepoint.portalName == portalName) {
            setPlayerToPosition(savepoint.transform.position);
            checkpoint = savepoint.transform;
            foundPortal = true;
            break;
          }
        }
      }

      // camp fire
      if (!foundPortal) {
        Checkpoint[] campfires = FindObjectsOfType<Checkpoint>();
        foreach (Checkpoint campfire in campfires) {
          if (campfire.portalName == portalName) {
            setPlayerToPosition(campfire.transform.position);
            checkpoint = campfire.transform;
            foundPortal = true;
            campfire.lit();
            break;
          }
        }
      }
      Debug.Assert(foundPortal, "Unable to find portal: " + portalName);

      // play background music if set
      GetComponent<MusicPlayer>()?.Play();
    }

    private void setPlayerToPosition(Vector3 position) {
      Transform playerTransform = GameObject.FindGameObjectWithTag(Common.Tags.PLAYER).transform;
      playerTransform.position = Common.Utils.overrideXy(playerTransform.position, position);
    }

    public void registerCheckpoint(Transform checkpoint) {
      Debug.Log("Register checkpoint: " + checkpoint.gameObject.name);
      this.checkpoint = checkpoint;
    }

    public void loadCheckpoint() {
      if (checkpoint == null) {
        Debug.LogWarning("No checkpoint has been registered.");
      }
      GameObject player = GameObject.FindGameObjectWithTag(Common.Tags.PLAYER);

      player.transform.position = checkpoint.position;
      Rigidbody2D playerRb2d = player.GetComponent<Rigidbody2D>();
      playerRb2d.velocity = Vector2.zero;
    }
  }
}

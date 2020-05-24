using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Fx {
  public class Ghost {
    public GameObject ghostGo;
    public float remainLifetimeSecond;
    public float lifetimeSecond;

    public bool isAlive {
      get => remainLifetimeSecond > 0f;
    }
    public Ghost(GameObject ghostGo, float lifetimeSecond) {
      this.ghostGo = ghostGo;
      this.lifetimeSecond = lifetimeSecond;
      this.remainLifetimeSecond = lifetimeSecond;
    }
  }

  [RequireComponent(typeof(SpriteRenderer))]
  public class GhostSprite : MonoBehaviour {

    public GameObject ghostPrefab;
    public SpriteRenderer[] targetSpriteRenderers;

    public bool emit = true;
    public bool useUnscaledTime = false;
    public float spawnZOffset = .01f;
    public float spawnInterval = .2f;
    public float ghostLifetimeSecond = 1f;
    public Color initialGhostColor = Color.black;
    public Color initialGhostAmbientColor = Color.white;
    public Color finalGhostColor = new Color(0f, 0f, 0f, 0f);
    public Color finalGhostAmbientColor = Color.white;

    [Header("Debug")]
    public int maxLivingGhosts = 10;
    private int createdGhostNumber = 0;

    private Queue<Ghost> idleGhosts = new Queue<Ghost>();
    private List<Ghost> livingGhosts = new List<Ghost>();
    private float spawnIntervalRemain;

    // Update is called once per frame
    void Update() {
      updateGhostSpawn();
      updateLivingGhost();
    }

    private void updateLivingGhost() {
      foreach (Ghost ghost in livingGhosts) {
        ghost.remainLifetimeSecond -= getDeltaTime();
        // ghost lifecycle attribute update
        ghost.ghostGo.GetComponent<SpriteRenderer>().material.SetColor(
          Materials.MATERIAL_ATTRIBUTE_TINT,
          Color.Lerp(finalGhostColor, initialGhostColor, ghost.remainLifetimeSecond / ghost.lifetimeSecond));
        ghost.ghostGo.GetComponent<SpriteRenderer>().material.SetColor(
          Materials.MATERIAL_ATTRIBUTE_AMBIENT,
          Color.Lerp(finalGhostAmbientColor, initialGhostAmbientColor, ghost.remainLifetimeSecond / ghost.lifetimeSecond));
        // return ghost
        if (!ghost.isAlive) {
          returnGhost(ghost);
        }
      }
      livingGhosts.RemoveAll(ghost => !ghost.isAlive);
    }

    private float getDeltaTime() => useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

    private void updateGhostSpawn() {
      if (spawnIntervalRemain > Mathf.Epsilon) {
        spawnIntervalRemain -= getDeltaTime();
        return;
      }
      spawnIntervalRemain += spawnInterval;
      foreach (SpriteRenderer spriteRenderer in targetSpriteRenderers) {
        spawnGhostIfNeeded(spriteRenderer);
      }
    }

    private void spawnGhostIfNeeded(SpriteRenderer spriteRenderer) {
      if (!spriteRenderer.enabled || !emit) {
        return;
      }
      Ghost ghost = getIdleGhost();
      GameObject ghostGo = ghost.ghostGo;
      // activeness
      ghost.remainLifetimeSecond = ghostLifetimeSecond;
      ghostGo.SetActive(true);
      // transform
      ghostGo.transform.position = spriteRenderer.gameObject.transform.position + new Vector3(0f, 0f, spawnZOffset);
      ghostGo.transform.rotation = spriteRenderer.gameObject.transform.rotation;
      ghostGo.transform.localScale = spriteRenderer.gameObject.transform.lossyScale;
      // spriteRenderer
      SpriteRenderer ghostRenderer = ghostGo.GetComponent<SpriteRenderer>();
      ghostRenderer.sprite = spriteRenderer.sprite;
    }

    private Ghost getIdleGhost() {
      if (idleGhosts.Count > 0) {
        Ghost reusedGhost = idleGhosts.Dequeue();
        livingGhosts.Add(reusedGhost);
        return reusedGhost;
      }
      if (livingGhosts.Count >= maxLivingGhosts) {
        Debug.LogError("Requiring more ghosts than allow number: " + maxLivingGhosts);
      }
      GameObject ghostGo = Instantiate(ghostPrefab);
      ghostGo.name = "Ghost[" + createdGhostNumber + "]";
      createdGhostNumber++;
      Ghost ghost = new Ghost(ghostGo, ghostLifetimeSecond);
      livingGhosts.Add(ghost);
      return ghost;
    }

    private void returnGhost(Ghost ghost) {
      ghost.ghostGo.SetActive(false);
      idleGhosts.Enqueue(ghost);
    }

    private void OnDestroy() {
      foreach (Ghost ghost in idleGhosts) {
        Destroy(ghost.ghostGo);
      }
      foreach (Ghost ghost in livingGhosts) {
        Destroy(ghost.ghostGo, ghost.remainLifetimeSecond);
      }
    }
  }
}

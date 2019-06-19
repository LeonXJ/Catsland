using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using Catsland.Scripts.Bullets;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  public class BreakableWall : MonoBehaviour, IDamageInterceptor {

    public int hp = 3;
    public float shakeTimeSecond = 0.5f;
    public float shakeA = 0.1f;
    public int genFragmentNumber = 5;
    public Vector2 fragmentInitVelocityXRange = new Vector2(-1.0f, 1.0f);
    public Vector3 shakeVelocity = Vector3.up;

    public GameObject[] fragmentGos;

    private int currentHp;
    private Vector3 originalPosition;

    private float shakeRemainTimeSecond = 0.0f;
    private BoxCollider2D boxCollider2d;

    void Awake() {
      boxCollider2d = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start() {
      currentHp = hp;
      originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
      if (shakeRemainTimeSecond > 0.0f) {
        float deltaX = Random.Range(-shakeA, shakeA);
        transform.position = originalPosition + new Vector3(deltaX, .0f, .0f);
        shakeRemainTimeSecond -= Time.deltaTime;
      }
    }
    public void damage(DamageInfo damageInfo) {
      currentHp -= damageInfo.damage;
      generateFragments(genFragmentNumber);
      UnityEngine.Camera.main.DOShakePosition(0.4f, new Vector2(0.1f, 0.0f), 10, 90, true);
      if (currentHp <= 0) {
        // destory
        SceneConfig.getSceneConfig().stonePillarShake(transform.position, shakeVelocity);
        Destroy(gameObject);
        return;
      }

      // Shake effect
      shakeRemainTimeSecond = shakeTimeSecond;
    }

    private void generateFragments(int num) {
      for (int i = 0; i < num; i++) {
        int index = (int)Random.Range(.0f, fragmentGos.Length);
        GameObject fragment = Instantiate(fragmentGos[index]);

        fragment.transform.position = 
          new Vector3(
            Random.Range(boxCollider2d.bounds.min.x, boxCollider2d.bounds.max.x),
            Random.Range(boxCollider2d.bounds.min.y, boxCollider2d.bounds.max.y),
            transform.position.z);

        Fragment fragmentComponent = fragment.GetComponent<Fragment>();
        fragmentComponent.ignoreGo = gameObject;

        Rigidbody2D rb2d = fragment.GetComponent<Rigidbody2D>();
        rb2d.angularVelocity = Random.Range(-180.0f, 180.0f);
        rb2d.velocity = new Vector3(Random.Range(fragmentInitVelocityXRange.x, fragmentInitVelocityXRange.y), 0.0f);

      }
    }

    ArrowResult IDamageInterceptor.getArrowResult(ArrowCarrier arrowCarrier) {
      return arrowCarrier.isShellBreaking ? ArrowResult.HIT_AND_BROKEN : ArrowResult.BROKEN;
    }
  }

}

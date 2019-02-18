using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;
using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.Controller {
  [RequireComponent(typeof(Animator)), RequireComponent(typeof(EvilFlowerInput))]
  public class EvilFlowerController: MonoBehaviour {

    public interface EvilFlowerInput {
      bool attack();
    }

    private static readonly string STATE_NAME_IDEAL = "Ideal";
    private static readonly string TRIGGER_SHOOT = "Shoot";

    public GameObject missilePrefab;
    public Transform missileGeneration;
    public float missileSpeed = 2.0f;
    public float missileLifetime = 3.0f;
    public float shootIntervalInSecond = 5.0f;
    public int health = 2;

    private Animator animator;
    private EvilFlowerInput input;
    private float lastShootTime;


    private void Awake() {
      animator = GetComponent<Animator>();
      input = GetComponent<EvilFlowerInput>();
    }


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
      if(input.attack() && CanShoot()) {
        animator.SetTrigger(TRIGGER_SHOOT);
      }

    }

    public bool CanShoot() {
      return animator.GetCurrentAnimatorStateInfo(0).IsName(STATE_NAME_IDEAL)
        && (Time.time - lastShootTime) > shootIntervalInSecond;
    }

    public void Shoot() {

      GameObject player = GameObject.FindGameObjectWithTag(Tags.PLAYER);

      GameObject missileGo = Instantiate(missilePrefab);
      missileGo.transform.position = missileGeneration.position;
      missileGo.transform.rotation = missileGeneration.rotation;

      Missile missile = missileGo.GetComponent<Missile>();
      missile.Fire(player, missileGeneration.up * missileSpeed, missileLifetime);
      lastShootTime = Time.time;
    }

    public void damage(DamageInfo damageInfo) {
      health -= damageInfo.damage;
      if(health <= 0) {
        enterDie();
      }
    }

    private void enterDie() {
      Destroy(gameObject);
    }
  }
}

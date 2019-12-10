using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Ai;
using Catsland.Scripts.Bullets;

namespace Catsland.Scripts.Controller {

  public class BeeHiveController: MonoBehaviour {


    public GameObject beePrefab;
    public Transform generationPosition;
    public float regenerationIntervalS = 5.0f;
    public Transform[] patrolPoints;
    public int generateBeeNumberWhenDestroy = 2;
    public bool initLockOn = false;

    public int maxHp = 5;
    private int currrentHp;


    private GameObject aliveBee;
    private float regenerationTick = 0.0f;


    // Start is called before the first frame update
    void Start() {
      currrentHp = maxHp;
    }

    // Update is called once per frame
    void Update() {
      if(aliveBee == null) {
        regenerationTick += Time.deltaTime;
        if(regenerationTick > regenerationIntervalS) {
          aliveBee = generateBee(initLockOn);
          regenerationTick = 0.0f;
        }
      }
    }

    private GameObject generateBee(bool lockOn) {
      GameObject bee = Instantiate(beePrefab);
      bee.transform.position = generationPosition.position;

      // Set patrol points
      BeeAi ai = bee.GetComponent<BeeAi>();
      ai.patrolPoints = patrolPoints;
      ai.lockOn = lockOn;
      return bee;
    }

    public void damage(DamageInfo damageInfo) {
      currrentHp -= damageInfo.damage;
      if(currrentHp <= 0) {
        enterDie();
      }
    }

    private void enterDie() {
      for(int i = 0; i < generateBeeNumberWhenDestroy; i++) {
        generateBee(true);
      }
      Destroy(gameObject);
    }
  }
}

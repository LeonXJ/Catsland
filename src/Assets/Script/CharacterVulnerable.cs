using UnityEngine;
using Catslandx.Script.Common;

namespace Catslandx {
  public class CharacterVulnerable :MonoBehaviour, IVulnerable {

    public int life = 1;
    private int currentLife = 1;
    public bool isCanGetHurt = true;
    public bool isCanGetRepel = true;

    private new Rigidbody2D rigidbody2D;
    private ICharacterController2D characterController;

    public void setCanGetHurt(bool canGetHurt) {
      isCanGetHurt = canGetHurt;
    } 

    public bool getCanGetHurt() {
      return isCanGetHurt;
    }

    public int getHurt(int hurtPoint, Vector2 repelForce) {
      if (isCanGetHurt) {
        currentLife -= hurtPoint;
        // get repel
        if (isCanGetRepel && rigidbody2D != null && characterController != null) {
          if(currentLife > 0) {
            characterController.getHurt(hurtPoint);
            //rigidbody2D.AddForce(repelForce);
            rigidbody2D.velocity = repelForce;
          } else {
            characterController.die();
          }
        }
      }
      return 0;
    }

    // Use this for initialization
    void Start() {
      rigidbody2D = GetComponent<Rigidbody2D>();
      characterController = GetComponent<ICharacterController2D>();
      currentLife = life;
    }

    // Update is called once per frame
    void Update() {

    }

    public void respawn() {
      currentLife = life;
    }
  }
}


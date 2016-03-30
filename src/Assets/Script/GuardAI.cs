using UnityEngine;
using System.Collections;

namespace Catslandx {
  [RequireComponent(typeof(CharacterController2D))]
  public class GuardAI :MonoBehaviour {

    public float viewDistance = 3.0f;
    public float viewAngelInDegree = 50.0f;
    public float maxPursueDistance = 3.0f;
    public LayerMask viewBlocker;

    private CharacterController2D characterController;
    private Vector2 initialPosition;


    // Use this for initialization
    void Start() {
      initialPosition = transform.position;
      characterController = GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update() {
      GameObject player = getPlayerInView();
      if (player != null) {
        Vector2 delta = player.transform.position - transform.position;
        float diviateDistance = transform.position.x - initialPosition.x;
        if ((diviateDistance >= maxPursueDistance && delta.x > 0.0f)
          || (diviateDistance <= -maxPursueDistance && delta.x < 0.0f)) {
          // just stand and watch
        } else {
          characterController.move(delta.x > 0.0f ? 1.0f : -1.0f, false, false, false);
        }
      } else {
        // travel
        float toOriginal = transform.position.x - initialPosition.x;
        if (Mathf.Abs(toOriginal) > 0.5f) {
          characterController.move(toOriginal > 0.0f ? -1.0f : 1.0f, false, false, false);
        }
      }
    }

    GameObject getPlayerInView() {
      GameObject player = GameObject.FindGameObjectWithTag("Player");
      if (player != null) {
        Vector2 delta = player.transform.position - transform.position;
        if (delta.magnitude < viewDistance) {
          Vector2 orientation = characterController.getIsFaceRight() ? Vector2.right : Vector2.left;
          if (Mathf.Abs(Vector2.Angle(orientation, delta)) < viewAngelInDegree) {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, delta, viewDistance, viewBlocker);
            if (hit.transform.gameObject == player) {
              return player;
            }
          }
        }
      }
      return null;
    }
  }
}


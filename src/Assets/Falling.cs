using UnityEngine;
using System.Collections;

namespace Catslandx {
  public class Falling : MonoBehaviour {

    public float speed = 0.3f;

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
      transform.position = transform.position + Vector3.down * speed;
    }
  }
}

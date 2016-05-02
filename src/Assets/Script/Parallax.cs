using UnityEngine;
using System.Collections;

namespace Catsland {
  public class Parallax : MonoBehaviour {
    
    public Vector2 scale = Vector2.one;

    private Vector2 initCameraPosition;
    private Vector2 initGameObjectPosition;

    // Use this for initialization
    void Start () {
      initCameraPosition = Camera.main.transform.position;
      initGameObjectPosition = transform.position;
    }
      
    // Update is called once per frame
    void Update () {
      Vector2 cameraCurrentPosition = Camera.main.transform.position;
      Vector2 cameraDelta = cameraCurrentPosition - initCameraPosition;
      Vector2 gameObjectPosition = initGameObjectPosition + MathHelper.multiple(cameraDelta, scale);
      transform.position = new Vector3(gameObjectPosition.x, gameObjectPosition.y, transform.position.z);
    }
  }
}


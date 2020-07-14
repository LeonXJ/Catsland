using UnityEngine;
using Catsland.Scripts.Controller;

namespace Catsland.Scripts.Camera {
  public class FocusPointController : StackEffectController<CameraOffsetConfig> {

    public float horizontalSmooth;
    public float verticalSmooth;
    public Vector2 baseOffset = Vector2.zero;
    public Vector2 defaultOffset = Vector2.zero;
    public float defaultChangeSpeed = 0.2f;

    private Vector2 velocity;

    // Update is called once per frame
    void Update() {
      transform.localPosition =
        Vector2.Lerp(transform.localPosition, baseOffset + selectedConfig.offset, selectedConfig.valueChangeSpeed * Time.deltaTime);
    }
  }
}

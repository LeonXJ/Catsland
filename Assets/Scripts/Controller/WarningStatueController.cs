using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class WarningStatueController : MonoBehaviour {

    public GameObject playerSensorGo;
    public Sprite untriggeredSprite;
    public Sprite triggerredSprite;

    private ISensor playerSensor;
    private SpriteRenderer renderer;

    void Awake() {
      renderer = GetComponent<SpriteRenderer>();
      playerSensor = playerSensorGo.GetComponent<ISensor>();
    }

    // Update is called once per frame
    void Update() {
      renderer.sprite = playerSensor.isStay() ? triggerredSprite : untriggeredSprite;
    }
  }
} 

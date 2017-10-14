using UnityEngine;
using System.Collections;

namespace Catslandx {
  public class SoundRipple : MonoBehaviour {

    private float currentTransmitDistance;
    public float currentVolume;
    private bool isFrozen = true;
    private SoundPackageInformation soundPackageInformation;

    public static bool isAudioSystemFrozen = false;
    public static float transmitSpeedSecond = 1.8f;
    public static float attenuationSecond = 0.8f;
    public static LayerMask soundReceiverLayer = LayerMask.GetMask("SoundReceiver");
    public static Color rippleColor = Color.blue;
    public static int segments = 32;

    private LineRenderer lineRenderer;
    
    private void initiate(float volume, SoundPackageInformation soundPackageInformation) {
      this.currentVolume = volume;
      this.currentTransmitDistance = 0.0f;
      this.soundPackageInformation = soundPackageInformation;
      this.isFrozen = false;
    }

    // Use this for initialization
    void Start () {
      lineRenderer = gameObject.AddComponent(typeof(LineRenderer)) as LineRenderer;
      lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
      lineRenderer.SetWidth(0.01f, 0.01f);
      lineRenderer.SetVertexCount(segments + 1);
    }

    // Update is called once per frame
    void Update () {
      if (isFrozen || isAudioSystemFrozen) {
        return;
      }
      float deltaTimeInS = Time.deltaTime;
      currentVolume -= deltaTimeInS * attenuationSecond;
      if (currentVolume < 0.0f) {
        selfDestory();
      }
      currentTransmitDistance += deltaTimeInS * transmitSpeedSecond;
      // broadcast to the receivers in the distance
      broadcast();

      drawCircle(transform.position, currentTransmitDistance);
    }

    private void broadcast() {
      Collider2D[] colliders = Physics2D.OverlapCircleAll(
        transform.position, currentTransmitDistance, soundReceiverLayer);
      foreach (Collider2D receiver in colliders) {
        ISoundReceiver soundReceiver = receiver.gameObject.GetComponent<ISoundReceiver>();
        if (soundReceiver != null) {
          soundReceiver.receive(currentVolume, soundPackageInformation);
        }
      }
    }

    private void selfDestory() {
      GameObject.Destroy(gameObject);
    }

    public static SoundRipple createRipple(float volume, Vector3 position, GameObject soundMaker) {
      GameObject ripple = new GameObject("Sound Ripple");
      ripple.transform.position = position;
      SoundRipple soundRipple = ripple.AddComponent(typeof(SoundRipple)) as SoundRipple;
      soundRipple.initiate(volume, new SoundPackageInformation(position, soundMaker));
      return soundRipple;  
    }

    public void drawCircle(Vector2 position, float radius) {
      float segmentAngle = 2.0f * Mathf.PI / segments;
      for (int segment = 0; segment <= segments; ++segment) {
        float angle = segmentAngle * segment;
        float x = position.x + radius * Mathf.Cos(angle);
        float y = position.y + radius * Mathf.Sin(angle);
        lineRenderer.SetPosition(segment, new Vector3(x, y, 0.0f));
      }
      rippleColor.a = currentVolume;
      lineRenderer.SetColors(rippleColor, rippleColor);
    }
  }
}

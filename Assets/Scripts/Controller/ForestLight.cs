using UnityEngine;

namespace Catsland.Scripts.Controller {
  public class ForestLight: MonoBehaviour {

    public float cloudyRatio = .3f;
    public float cloudMinLength = 3f;
    public float cloudMaxLength = 10f;
    public float cloudSpeed = 1f;
    public float cloudMaxDense = 1f;
    public float cloudMinDense = 0.0f;

    private SpriteRenderer spriteRender;

    private float currentPeakDense = 0.0f;
    private float currentCloudLength = 0.0f;
    private float currentPeakPosition = 0.0f;
    private float currentPosition = 0.0f;


    // Use this for initialization
    void Start() {
      spriteRender = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
      currentPosition += cloudSpeed * Time.deltaTime;
      if(currentPosition > currentCloudLength) {
        randomCloud();
      }
      // calculate dense
      float currentDense = 0.0f;
      if(currentPosition < currentPeakPosition) {
        if(currentPeakPosition > 0.0f) {
          currentDense = Mathf.Lerp(cloudMinDense, currentPeakDense, currentPosition / currentPeakPosition);
        }
      } else {
        if(currentPosition > currentPeakPosition) {
          currentDense = Mathf.Lerp(currentPeakDense, cloudMinDense, (currentPosition - currentPeakPosition) / (currentCloudLength - currentPeakPosition));
        }
      }
      if(spriteRender != null) {
        Color originalColor = spriteRender.material.color;
        spriteRender.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f - currentDense);
      }
    }

    void randomCloud() {
      float random = Random.value;
      if(random < cloudyRatio) {
        generateCloud(cloudMaxDense);
      } else {
        generateCloud(cloudMinDense);
      }
    }

    void generateCloud(float maxDense) {
      currentCloudLength = Random.Range(cloudMinLength, cloudMaxLength);
      currentPeakPosition = Random.Range(currentPeakPosition * 0.2f, currentCloudLength * 0.8f);
      currentPeakDense = Random.Range(cloudMinDense, maxDense);
      currentPosition = 0.0f;
    }
  }
}

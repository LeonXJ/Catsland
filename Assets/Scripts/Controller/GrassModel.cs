using UnityEngine;
using Catsland.Scripts.Misc;

namespace Catsland.Scripts.Controller {
  [RequireComponent(typeof(MeshRenderer))]
  [RequireComponent(typeof(MeshFilter))]
  [ExecuteInEditMode]
  public class GrassModel: MonoBehaviour {

    public Texture texture;
    public float width = 1.0f;
    public float height = 0.3f;
    public float degree = 0.0f;

    // TODO: use global setting.
    public int pixelPerUnit = 25;

    // Stop updating grass swinging if the camera is further than this distance.
    public float stopUpdateDistance = 10.0f;

    // Whether and how the grass is pressed.
    public enum PressStatus {
      NONE = 0,
      LEFT = 1,
      RIGHT = 2,
    }
    public PressStatus pressStatus = PressStatus.NONE;
    public float maxSwingSpeed = 45.0f;

    // Following attributes are used when pressStatus is NONE.
    // Phase of the swing cycle.
    // Swing as: 
    //   degree = center + Sin(phase) * amp
    //   amp = maxDegree - | center |
    public float phase = 0.0f;
    public float maxDegree = 45.0f;
    public float center = 0.0f;
    public float frequency = 10.0f;

    // Max degree when pressStatus is LEFT/RIGHT.
    public float maxPressDegree = 80.0f;

    private bool hasInitialized = false;
    private Vector3[] vertices;

    void Update() {
      if(!hasInitialized) {
        initializeMesh();
      }
      if(Application.isPlaying && isInMainCamera()) {
        updateDegree();
        updateVertices();
        updateTexture();
      }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
      Rigidbody2D rigidbody = collision.GetComponent<Rigidbody2D>();
      if(rigidbody != null) {
        pressStatus = rigidbody.velocity.x > 0.0f ? PressStatus.RIGHT : PressStatus.LEFT;
      }
    }

    private void OnTriggerExit2D(Collider2D collision) {
      pressStatus = PressStatus.NONE;
    }

    private void updateTexture() {
      MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
      meshRenderer.sharedMaterial.mainTexture = texture;
    }

    private void initializeMesh() {
      MeshFilter meshFilter = GetComponent<MeshFilter>();
      Mesh mesh = new Mesh();
      meshFilter.sharedMesh = mesh;

      vertices = new Vector3[4];

      vertices[0] = new Vector3(-width / 2.0f, 0.0f, 0.0f);
      vertices[1] = new Vector3(width / 2.0f, 0.0f, 0.0f);
      vertices[2] = new Vector3(-width / 2.0f, height, 0.0f);
      vertices[3] = new Vector3(width / 2.0f, height, 0.0f);

      mesh.vertices = vertices;

      int[] triangles = new int[6];

      triangles[0] = 0;
      triangles[1] = 2;
      triangles[2] = 1;

      triangles[3] = 2;
      triangles[4] = 3;
      triangles[5] = 1;

      mesh.triangles = triangles;

      Vector3[] normals = new Vector3[4];

      normals[0] = -Vector3.forward;
      normals[1] = -Vector3.forward;
      normals[2] = -Vector3.forward;
      normals[3] = -Vector3.forward;

      mesh.normals = normals;

      Vector2[] uv = new Vector2[4];

      uv[0] = new Vector2(0.0f, 0.0f);
      uv[1] = new Vector2(1.0f, 0.0f);
      uv[2] = new Vector2(0.0f, 1.0f);
      uv[3] = new Vector2(1.0f, 1.0f);

      mesh.uv = uv;

      hasInitialized = true;
    }

    private void updateVertices() {

      Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

      vertices[0] = new Vector3(-width * 0.5f, 0.0f, 0.0f);
      vertices[1] = new Vector3(width * 0.5f, 0.0f, 0.0f);
      Vector3 farendBias = new Vector3(
        height * Mathf.Sin(Mathf.Deg2Rad * degree),
        height * Mathf.Cos(Mathf.Deg2Rad * degree),
        0.0f);
      vertices[2] = vertices[0] + farendBias;
      vertices[3] = vertices[1] + farendBias;

      mesh.vertices = vertices;
      mesh.RecalculateBounds();
    }

    public void UpdateSize() {
      if(texture != null) {
        width = (float)texture.width / pixelPerUnit;
        height = (float)texture.height / pixelPerUnit;
      }
    }

    private void updateDegree() {
      float targetDegree = degree;
      if(pressStatus == PressStatus.NONE) {
        phase += Time.deltaTime * frequency;
        phase -= 360.0f * Mathf.Floor(phase / 360.0f);

        float realAmp = maxDegree - Mathf.Min(Mathf.Abs(center), maxDegree);
        //degree = Mathf.Lerp(degree, center + Mathf.Sin(Mathf.Deg2Rad * phase) * realAmp, Time.deltaTime * frequency);
        targetDegree = center + Mathf.Sin(Mathf.Deg2Rad * phase) * realAmp;
      } else {
        targetDegree = pressStatus == PressStatus.LEFT ? -maxPressDegree : maxPressDegree;
      }
      applyDegreeChange(targetDegree);
    }

    private void applyDegreeChange(float targetDegree) {
      float delta = targetDegree - degree;
      float confinedDelta = Mathf.Clamp(delta, -maxSwingSpeed * Time.deltaTime, maxSwingSpeed * Time.deltaTime);
      degree += confinedDelta;
    }

    public void OnAttributeUpdate() {
      updateVertices();
      updateTexture();
    }

    private bool isInMainCamera() {
      Vector3 cameraPosition = SceneConfig.getSceneConfig().MainCamera.transform.position;
      return Vector2.SqrMagnitude(cameraPosition - transform.position) < stopUpdateDistance * stopUpdateDistance;
    }
  }
}

using System.Collections.Generic;
using UnityEngine;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Controller {
  [RequireComponent(typeof(MeshRenderer))]
  [RequireComponent(typeof(MeshFilter))]
  [ExecuteInEditMode]
  public class GrassModel : MonoBehaviour {

    // The center of swing.
    public enum SwingCenter {
      BOTTOM,
      TOP,
    }

    // Whether and how the grass is pressed.
    public enum PressStatus {
      NONE = 0,
      LEFT = 1,
      RIGHT = 2,
    }

    // Mesh and sprite attributes.
    public Sprite sprite;
    public Material material;
    public float width = 1.0f;
    public float height = 0.3f;
    public int heightSegment = 2;


    private Vector3[] vertices;

    // Movement attributes.
    public SwingCenter swingCenter = SwingCenter.BOTTOM;

    // Value range of centerDegree.
    public float maxCenterDegree = 80.0f;

    // Center restoring speed degree per second.
    public float centerRestoringDegreeSpeed = 45.0f;

    // Range of frequency, ~swingCenter.
    public Vector2 frequencyRange = new Vector2(100.0f, 1000.0f);

    // Range of swing amp in degree, ~1/swingCenter.
    public Vector2 ampRangeDegree = new Vector2(5.0f, 20.0f);

    // Grass swings around center. 0.0 means middle. Value is within CenterRange.
    // for debugging
    public float centerDegree = 0.0f;

    // If set, particle will be shot on trigger.
    public ParticleSystem particleSystem;

    // The min internal between two particle shoot.
    public float particleEmitMinInternalSecond = 1f;


    // frequency = [minFrequency, maxFrequency] ~ centerDegree
    private float frequency = 10.0f;

    // Following attributes are used when pressStatus is NONE.
    // Degree of swing is calculate as:
    // 1. phase = frequency * t
    // 2. target_degree = centerDegree + Sin(phase) * amp
    // 3. degree = lerp(target_degree, degree)

    // phase is in range of [.0f, 360.0f]
    private float phase = 0.0f;
    private float degree = 0.0f;

    // The winds which affect this grass.
    // for debugging
    public HashSet<IWind> winds = new HashSet<IWind>();

    private PressStatus pressStatus = PressStatus.NONE;

    private float particleCd = .0f;


    // Movement updating attributes.

    // Whether to assign a random init phase.
    public bool randomInitPhase = true;

    // Stop updating grass swinging if the camera is further than this distance.
    public float stopUpdateDistance = 10.0f;

    // Whether the mesh has been created.
    private bool hasInitialized = false;

    // How fast is the degree changes towards target degree.
    public float maxSwingSpeed = 45.0f;

    // Max degree when pressStatus is LEFT/RIGHT.
    public float maxPressDegree = 80.0f;

    private void Start() {
      if (randomInitPhase) {
        phase = Random.Range(0.0f, 360.0f);
      }
    }

    private Material GetMaterial() => material;

    void Update() {
      if (!hasInitialized) {
        initializeMesh();
      }
      if (Application.isPlaying && isInMainCamera()) {
        updateWinds();
        updateDegree();
        updateVertices();
        updateTexture();
      }
      if (particleSystem != null) {
        if (particleCd > .0f) {
          particleCd -= Time.deltaTime;
        }
      }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
      IWind wind = collision.gameObject.GetComponent<IWind>();
      if (wind != null && !winds.Contains(wind)) {
        winds.Add(wind);
        return;
      }
      Rigidbody2D rigidbody = collision.GetComponent<Rigidbody2D>();
      if (rigidbody != null) {
        pressStatus = rigidbody.velocity.x > 0.0f ? PressStatus.RIGHT : PressStatus.LEFT;
      }
      // particle effect
      if (particleSystem != null && particleCd <= .0f) {
        particleSystem.Play();
        particleCd = particleEmitMinInternalSecond;
      }
    }

    private void OnTriggerExit2D(Collider2D collision) {
      IWind wind = collision.gameObject.GetComponent<IWind>();
      if (wind != null && winds.Contains(wind)) {
        winds.Remove(wind);
        return;
      }
      pressStatus = PressStatus.NONE;
    }

    private void updateTexture() {
      MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
      if (meshRenderer.sharedMaterial == null || meshRenderer.sharedMaterial.name != sprite.name) {
        meshRenderer.sharedMaterial = GetMaterial();
      }
      Debug.Assert(meshRenderer.sharedMaterial != null, "Material is null for object: " + gameObject.name);
      meshRenderer.sharedMaterial.mainTexture = sprite.texture;
    }

    private void initializeMesh() {
      MeshFilter meshFilter = GetComponent<MeshFilter>();
      Mesh mesh = new Mesh();
      meshFilter.sharedMesh = mesh;

      int vertexCount = (heightSegment + 1) * 2;
      //vertices = new Vector3[4];
      //Vector3[] normals = new Vector3[4];
      vertices = new Vector3[vertexCount];
      Vector3[] normals = new Vector3[vertexCount];
      Vector2[] uvs = new Vector2[vertexCount];

      float halfWidth = width * 0.5f;
      float segmentHeight = height / heightSegment;

      Vector2 leftBottomUv = sprite.uv[2];
      Vector2 rightBottomUv = sprite.uv[3];
      Vector2 leftTopUv = sprite.uv[0];

      float ky = 1.0f;
      float uvYStart = leftBottomUv.y;
      float uvYEnd = leftTopUv.y;

      switch (swingCenter) {
        case SwingCenter.BOTTOM:
        ky = 1.0f;
        uvYStart = leftBottomUv.y;
        uvYEnd = leftTopUv.y;
        break;
        case SwingCenter.TOP:
        ky = -1.0f;
        uvYStart = leftTopUv.y;
        uvYEnd = leftBottomUv.y;
        break;
      }

      for (int i = 0; i < (heightSegment + 1); i++) {
        float y = ky * segmentHeight * i;
        vertices[i * 2] = new Vector3(-halfWidth, y, 0.0f);
        vertices[i * 2 + 1] = new Vector3(halfWidth, y, 0.0f);

        normals[i * 2] = -Vector3.forward;
        normals[i * 2 + 1] = -Vector3.forward;

        float ratio = (float)i / heightSegment;
        float uvY = Mathf.Lerp(uvYStart, uvYEnd, ratio);
        uvs[i * 2] = new Vector2(leftBottomUv.x, uvY);
        uvs[i * 2 + 1] = new Vector2(rightBottomUv.x, uvY);
      }

      mesh.vertices = vertices;
      mesh.normals = normals;
      mesh.uv = uvs;

      int triangleCount = heightSegment * 2;
      int[] triangles = new int[triangleCount * 3];

      for (int i = 0; i < heightSegment; i++) {
        int baseVertexIndex = i * 2;
        int baseTriangleIndex = i * 2 * 3;

        // left top triangle
        triangles[baseTriangleIndex] = baseVertexIndex + 2;
        triangles[baseTriangleIndex + 1] = baseVertexIndex;
        triangles[baseTriangleIndex + 2] = baseVertexIndex + 3;

        // Right bottom triangle
        triangles[baseTriangleIndex + 3] = baseVertexIndex;
        triangles[baseTriangleIndex + 4] = baseVertexIndex + 1;
        triangles[baseTriangleIndex + 5] = baseVertexIndex + 3;
      }

      mesh.triangles = triangles;
      hasInitialized = true;
    }

    private void updateVertices() {

      Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

      float ky = swingCenter == SwingCenter.BOTTOM ? 1.0f : -1.0f;
      float segmentLength = height / heightSegment;
      float halfWidth = width * 0.5f;
      for (int i = 0; i < heightSegment + 1; i++) {
        float length = segmentLength * i;
        float ratio = (float)i / heightSegment;
        float segDegree = Mathf.Lerp(centerDegree, degree, ratio);

        Vector3 offset = new Vector3(
          length * Mathf.Sin(Mathf.Deg2Rad * segDegree),
          ky * length * Mathf.Cos(Mathf.Deg2Rad * segDegree),
          0.0f);
        vertices[i * 2] = new Vector3(-halfWidth, 0.0f, 0.0f) + offset;
        vertices[i * 2 + 1] = new Vector3(halfWidth, 0.0f, 0.0f) + offset;

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
      }
    }

    private void updateWinds() {
      // Calcuate total wind force.
      float power = 0.0f;
      winds.RemoveWhere(wind => wind == null || !wind.IsAlive());
      foreach (IWind wind in winds) {
        power += wind.GetWindPower();
      }

      if (power * power > 0.01f) {
        float absPower = Mathf.Abs(power);
        float absCenter = Mathf.Min(absPower, maxCenterDegree);
        centerDegree = Mathf.Sign(power) * absCenter;
        frequency = Mathf.Lerp(frequencyRange.x, frequencyRange.y, absCenter / maxCenterDegree);
      } else {
        // No wind, restoring centerDegree.
        centerDegree = Mathf.Lerp(centerDegree, 0.0f, centerRestoringDegreeSpeed * Time.deltaTime);
        frequency = Mathf.Lerp(frequencyRange.x, frequencyRange.y, Mathf.Abs(centerDegree) / maxCenterDegree);
      }
    }

    public void UpdateSize() {
      if (sprite != null) {
        width = sprite.rect.width / sprite.pixelsPerUnit;
        height = sprite.rect.height / sprite.pixelsPerUnit;
      }
    }

    private void updateDegree() {
      float targetDegree = degree;
      if (pressStatus == PressStatus.NONE) {
        phase += Time.deltaTime * frequency;
        phase -= 360.0f * Mathf.Floor(phase / 360.0f);

        float realAmp = Mathf.Lerp(ampRangeDegree.x, ampRangeDegree.y, 1.0f - Mathf.Abs(centerDegree) / maxCenterDegree);
        targetDegree = centerDegree + Mathf.Sin(Mathf.Deg2Rad * phase) * realAmp;
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

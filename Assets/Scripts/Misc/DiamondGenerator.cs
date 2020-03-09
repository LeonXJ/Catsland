using UnityEngine;

namespace Catsland.Scripts.Misc {
  public class DiamondGenerator : MonoBehaviour {

    [System.Serializable]
    public class DiamondPrefabConfig {
      public GameObject diamondPrefab;
      public int number;
      public float initialSpeed = 1f;
      public float initialSpeedRandomRange = 0.5f;
    }

    public DiamondPrefabConfig[] diamondPrefabConfigs;

    public GameObject diamondPrefab;
    public float explosionSpeed = 1.0f;
    public float initialSpeedDirectionRange = 45f;

    [Header("Debug")]
    public bool debugEnable = false;

    public void Generate(int num, int value) {
      Debug.Assert(diamondPrefab != null, "Diamond prefab must be assigned.");

      if (diamondPrefabConfigs != null && diamondPrefabConfigs.Length > 0) {
        GenerateDiamond();
        return;
      }

      for (int i = 0; i < num; i++) {
        GameObject diamond = Instantiate(diamondPrefab);
        diamond.transform.position = transform.position;

        diamond.GetComponent<Diamond>().value = value;

        // Throw to random direction
        float angle = Random.Range(-Mathf.PI, Mathf.PI);
        diamond.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * explosionSpeed;
      }
    }

    public void GenerateDiamond() {
      Debug.Assert(diamondPrefabConfigs != null && diamondPrefabConfigs.Length > 0, "No diamond prefab is set.");
      foreach (DiamondPrefabConfig config in diamondPrefabConfigs) {
        for (int i = 0; i < config.number; i++) {
          GameObject diamond = Instantiate(config.diamondPrefab);
          diamond.transform.position = transform.position;
          float initialDirection = Random.Range(-initialSpeedDirectionRange, initialSpeedDirectionRange);

          Rigidbody2D rb2d = diamond.GetComponent<Rigidbody2D>();
          float initialSpeed = config.initialSpeed + Random.Range(-config.initialSpeedRandomRange, config.initialSpeedRandomRange);
          rb2d.velocity = new Vector2(Mathf.Sin(Mathf.Deg2Rad * initialDirection), Mathf.Cos(Mathf.Deg2Rad * initialDirection)) * initialSpeed;
        }
      }
    }

    void Update() {
      if (Input.GetKeyDown(KeyCode.Alpha3)) {
        Debug.Log("Debug DimamondGenerator> Generate diamonds.");
        GenerateDiamond();
      }
    }
  }
}





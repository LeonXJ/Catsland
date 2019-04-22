using UnityEngine;

namespace Catsland.Scripts.Misc {
  public class DiamondGenerator : MonoBehaviour {

    public GameObject diamondPrefab;
    public float explosionSpeed = 1.0f;

    public void Generate(int num, int value) {
      Debug.Assert(diamondPrefab != null, "Diamond prefab must be assigned.");
      for (int i = 0; i < num; i++) {
        GameObject diamond = Instantiate(diamondPrefab);
        diamond.transform.position = transform.position;

        diamond.GetComponent<Diamond>().value = value;

        // Throw to random direction
        float angle = Random.Range(-Mathf.PI, Mathf.PI);
        diamond.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * explosionSpeed;
      }
    }
  }
}





using UnityEngine;

namespace Catsland.Scripts.Common {
  public class Layers {
    private static readonly string LAYER_GROUND_NAME = "Ground";
    private static readonly string LAYER_CHARACTER_NAME = "Character";
    private static readonly string LAYER_BULLET_NAME = "Bullet";

    public static readonly int LayerGround = LayerMask.NameToLayer(LAYER_GROUND_NAME);
    public static readonly int LayerCharacter = LayerMask.NameToLayer(LAYER_CHARACTER_NAME);
    public static readonly int LayerBullet = LayerMask.NameToLayer(LAYER_BULLET_NAME);
  }
}

using UnityEngine;

namespace Catsland.Scripts.Common {
  public class Layers {
    private static readonly string LAYER_GROUND_NAME = "Ground";
    private static readonly string LAYER_CHARACTER_NAME = "Character";
    private static readonly string LAYER_BULLET_NAME = "Bullet";
    private static readonly string LAYER_DECORATION = "Decoration";
    private static readonly string LAYER_LIGHT = "Light";
    private static readonly string LAYER_VULNERABLE_UNSOLID = "VulnerableUnsolid";
    private static readonly string LAYER_VULNERABLE_OBJECT = "VulnerableObject";

    public static readonly int LayerGround = LayerMask.NameToLayer(LAYER_GROUND_NAME);
    public static readonly int LayerCharacter = LayerMask.NameToLayer(LAYER_CHARACTER_NAME);
    public static readonly int LayerBullet = LayerMask.NameToLayer(LAYER_BULLET_NAME);
    public static readonly int LayerDecoration = LayerMask.NameToLayer(LAYER_DECORATION);
    public static readonly int LayerLight = LayerMask.NameToLayer(LAYER_LIGHT);
    public static readonly int LayerBeTrigger = LayerMask.NameToLayer(LAYER_VULNERABLE_UNSOLID);
    public static readonly int LayerVulnerableObject = LayerMask.NameToLayer(LAYER_VULNERABLE_OBJECT);
  }
}

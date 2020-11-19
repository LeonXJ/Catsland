using UnityEngine;

namespace Catsland.Scripts.Common {
  public class Layers {
    public const string LAYER_GROUND_NAME = "Ground";
    public const string LAYER_CHARACTER_NAME = "Character";
    public const string LAYER_BULLET_NAME = "Bullet";
    public const string LAYER_DECORATION = "Decoration";
    public const string LAYER_LIGHT = "Light";
    public const string LAYER_VULNERABLE_UNSOLID = "VulnerableUnsolid";
    public const string LAYER_VULNERABLE_OBJECT = "VulnerableObject";
    public const string LAYER_SELF_ILLUMINATE = "Self-illuminate";

    public static readonly int LayerGround = LayerMask.NameToLayer(LAYER_GROUND_NAME);
    public static readonly int LayerCharacter = LayerMask.NameToLayer(LAYER_CHARACTER_NAME);
    public static readonly int LayerBullet = LayerMask.NameToLayer(LAYER_BULLET_NAME);
    public static readonly int LayerDecoration = LayerMask.NameToLayer(LAYER_DECORATION);
    public static readonly int LayerLight = LayerMask.NameToLayer(LAYER_LIGHT);
    public static readonly int LayerBeTrigger = LayerMask.NameToLayer(LAYER_VULNERABLE_UNSOLID);
    public static readonly int LayerVulnerableObject = LayerMask.NameToLayer(LAYER_VULNERABLE_OBJECT);
    public static readonly int LayerSelfIlluminate = LayerMask.NameToLayer(LAYER_SELF_ILLUMINATE);
  }
}

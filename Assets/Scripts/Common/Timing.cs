using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Timing", order = 1)]
public sealed class Timing : ScriptableObject {

  public float DashKnockFreezeTime = 1f;
  public float EnemyHitShakeTime = .2f;
  public float DiamondDisappearTime = 2f;
  public float ArrowShakeTime = .1f;
}

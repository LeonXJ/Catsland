using UnityEngine;
using Panda;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Ai {
  public class Utils {

    public delegate void PerformAction();
    private GameObject characterGo;

    public Utils(GameObject characterGo) {
      this.characterGo = characterGo;
    }

    public Vector2 getDistanceToPlayer() {
      return SceneConfig.getSceneConfig().GetPlayer().transform.position - characterGo.transform.position;
    }

    public void setTaskSucceedIfPlayerInDistanceOrFail(Vector2 maxDistance) {
      Vector2 delta = getDistanceToPlayer();
      if(Mathf.Abs(delta.x) < maxDistance.x && Mathf.Abs(delta.y) < maxDistance.y) {
        Task.current.Succeed();
      } else {
        Task.current.Fail();
      }
    }

    public void setTaskSucceedIfPlayerInDistanceOrFail(float radius) {
      Vector2 delta = getDistanceToPlayer();
      if(Vector2.SqrMagnitude(delta) < radius * radius) {
        Task.current.Succeed();
      } else {
        Task.current.Fail();
      }
    }

    public static void succeedOnConditionElseDo(bool condition, PerformAction action) {
      if(condition) {
        Task.current.Succeed();
      } else {
        action();
      }
    }


  }
}

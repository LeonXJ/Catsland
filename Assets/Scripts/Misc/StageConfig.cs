using UnityEngine;
using UnityEngine.Playables;

namespace Catsland.Scripts.Misc {
  public class StageConfig : MonoBehaviour {

    public string stageName = "Untitled Stage";

    public GameObject[] opponents;

    public PlayableDirector stageStartDirector;
  }
}

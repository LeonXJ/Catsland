using System.Collections;
using UnityEngine;
using Catsland.Scripts.Common;
using Catsland.Scripts.Controller;
using UnityEngine.UI;

namespace Catsland.Scripts.Dialog {
  public class DialogTrigger: MonoBehaviour {

    public GameObject dialogSensorGo;
    public GameObject canTalkSignGo;
    public bool canTalk = false;
    public Text textArea;
    public float typeIntervalSecond = 0.02f;

    public string[] dialogs;

    private int currentDialogIndex = -1;
    private bool hasCurrentDialogIndexShown = false;

    private ISensor dialogSensor;
    private bool isPlayerInArea;
    private bool inTalkMode = false;

    private GameObject playerGoCache;
    private GameObject playerGo {
      get {
        if(playerGoCache == null) {
          playerGoCache = GameObject.FindGameObjectWithTag(Tags.PLAYER);
        }
        return playerGoCache;
      }
    }

    void Awake() {
      dialogSensor = dialogSensorGo.GetComponent<ISensor>();
    }

    void Update() {
      float horizontalDelta = playerGo.transform.position.x - transform.position.x;
      float playerOrientation = playerGo.GetComponent<PlayerController>().getOrientation();
      if(dialogSensor.getTriggerGos().Contains(playerGo)
        && horizontalDelta * playerOrientation < 0.0f) {
        // player faces to the NPC
        canTalkSignGo.SetActive(true);
        canTalk = true;
      } else {
        canTalkSignGo.SetActive(false);
        canTalk = false;
        if(inTalkMode) {
          exitDialog();
        }
      }

      // on player talk
      if(canTalk && playerGo.GetComponent<IInput>().interact()) {
        // enter player talk mode
        // start dialog
        if(currentDialogIndex < 0) {
          currentDialogIndex = 0;
          hasCurrentDialogIndexShown = false;
          inTalkMode = true;
          // set text
          StartCoroutine(type(dialogs[currentDialogIndex]));
          return;
        }
        // update dialog
        if(hasCurrentDialogIndexShown) {
          currentDialogIndex += 1;
          if(currentDialogIndex == dialogs.Length) {
            // end of dialog
            exitDialog();
          } else {
            // set text
            hasCurrentDialogIndexShown = false;
            StartCoroutine(type(dialogs[currentDialogIndex]));
          }
        }
      }
    }

    private void exitDialog() {
      inTalkMode = false;
      currentDialogIndex = -1;
      hasCurrentDialogIndexShown = false;
      textArea.text = "";
    }

    IEnumerator type(string text) {
      textArea.text = "";
      foreach(char letter in text) {
        // break the text
        if(!inTalkMode) {
          break;
        }
        textArea.text += letter;
        yield return new WaitForSeconds(typeIntervalSecond);
        if(playerGo.GetComponent<IInput>().interact()) {
          textArea.text = text;
          break;
        }
      }
      if(inTalkMode) {
        hasCurrentDialogIndexShown = true;
      }
    }
  }
}

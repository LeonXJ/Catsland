using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Catsland.Scripts.Controller;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.Ui {
  public class PauseMenu : MonoBehaviour {

    private const string MAIN_MENU_SCENE_NAME = "MainMenu";
    private static readonly string IS_PAUSE_MENU_SHOWN = "IsPauseMenuShown";

    public TimeScaleConfig timeScaleConfig;

    public Text[] menuOptions;
    public Text cursor;
    public float cursorPending = 30f;

    private bool isMenuShown = false;


    // Selected index in menuOptions.
    private int currentSelectedIndex = 0;

    private InputMaster inputMaster;
    private Animator animator;
    private TimeScaleController timeScaleController;

    private void Awake() {
      DontDestroyOnLoad(gameObject);
      inputMaster = new InputMaster();
    }

    // Start is called before the first frame update
    void Start() {
      animator = GetComponent<Animator>();
      timeScaleController = GameObject.FindGameObjectWithTag(Tags.TIME_SCALE_CONTROLLER)
        ?.GetComponent<TimeScaleController>();
      inputMaster.General.Option.performed += _ => {
        // Not show pause menu in main menu.
        if (SceneManager.GetActiveScene().name == MAIN_MENU_SCENE_NAME) {
          return;
        }

        if (isMenuShown) {
          HideMenu();
        } else {
          ShowMenu();
        }
      };
      inputMaster.General.MenuNext.performed += _ => {
        if (isMenuShown) {
          OnMenuNext();
        }
      };
      inputMaster.General.MenuPrevious.performed += _ => {
        if (isMenuShown) {
          OnMenuPrevious();
        }
      };
      inputMaster.General.Shoot.performed += _ => {
        // Warning: hacky
        if (currentSelectedIndex == 0) {
          HideMenu();
          return;
        }
        if (currentSelectedIndex == 1) {
          Debug.Log("Quit application.");
          Application.Quit();
        }
      };
    }

    // Update is called once per frame
    void Update() {

    }

    private void FixedUpdate() {
      
    }

    private void OnMenuNext() {
      currentSelectedIndex = (currentSelectedIndex + 1) % menuOptions.Length;
      UpdateCursorPosition();
    }

    private void OnMenuPrevious() {
      currentSelectedIndex = (currentSelectedIndex - 1) % menuOptions.Length;
      UpdateCursorPosition();
    }

    private void UpdateCursorPosition() {
      Text selectedOption = menuOptions[currentSelectedIndex];
      cursor.transform.position = selectedOption.transform.position - new Vector3(cursorPending, 0f, 0f);
    }

    private void ShowMenu() {
      currentSelectedIndex = 0;
      isMenuShown = true;

      // Time scale.
      timeScaleController.RegisterConfig(timeScaleConfig);
      animator.SetBool(IS_PAUSE_MENU_SHOWN, true);

      // Mute user input.
      DeviceInput input = GameObject.FindGameObjectWithTag(Tags.PLAYER)?.GetComponent<DeviceInput>();
      input.enabled = false;
    }

    private void HideMenu() {
      isMenuShown = false;
      animator.SetBool(IS_PAUSE_MENU_SHOWN, false);
      DeviceInput input = GameObject.FindGameObjectWithTag(Tags.PLAYER)?.GetComponent<DeviceInput>();
      input.enabled = true;
    }

    public void OnHideMenuComplete() {
      timeScaleController.UnregisterConfig(timeScaleConfig);
    }

    private void OnEnable() {
      inputMaster.Enable();
    }

    private void OnDisable() {
      inputMaster.Disable();
    }
  }
}


using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PauseMenu : MonoBehaviour
{
    //controls the pause and settings menus
    [Header("Menu References")]
    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private GameObject menuObj; //the pause menu object
    [SerializeField] private GameObject fadeObj; //the background fade object
    [SerializeField] private GameObject settingsObj; //the settings menu object

    [SerializeField] private GameObject defaultSelectSettings; //the default object to select when opening settings
    [SerializeField] private GameObject defaultSelectPause; //the default object to select when opening pause

    [Space, Header("Settings")]
    [SerializeField] private Slider sensSlider;
    private PlayerSettings playerSettings;

    private PauseManager pauseManager;

    private PlayerInputDetection playerInput;


    private void Start()
    {
        pauseManager = PauseManager.Instance;

        pauseManager.onGamePause += OpenPauseMenu;
        pauseManager.onGameResume += ClosePauseMenu;
    }

    private void OnEnable()
    {
        if (pauseManager != null)
        {
            pauseManager.onGamePause += OpenPauseMenu;
            pauseManager.onGameResume += ClosePauseMenu;
        }

    }

    private void OnDisable()
    {
        pauseManager.onGamePause -= OpenPauseMenu;
        pauseManager.onGameResume -= ClosePauseMenu;
    }

    #region Called On UI Interact
    public void OnResumePress()
    {
        pauseManager.UnpauseGame();
    }

    public void OnSettingsPress()
    {
        OpenSettingsMenu();
    }

    public void OnSettingsExit()
    {
        CloseSettingsMenu();
    }
    #endregion

    public void OnSliderChange(Slider slider)
    {
        playerSettings.SetSensitivity(playerInput.playerNum, (slider.value / slider.maxValue) * 300);
    }

    public void OnInvertCameraToggle(Toggle toggle)
    {
        playerSettings.SetCameraInvert(playerInput.playerNum, toggle.isOn);
    }



    private void OpenPauseMenu()
    {
        playerSettings = PlayerSettings.instance;
        fadeObj.SetActive(true);
        menuObj.SetActive(true);
        playerInput = pauseManager.playerInControl;
        eventSystem.SetSelectedGameObject(defaultSelectPause);

    }

    private void ClosePauseMenu()
    {
        fadeObj.SetActive(false);
        menuObj.SetActive(false);
        settingsObj.SetActive(false);
    }


    private void OpenSettingsMenu()
    {
        menuObj.SetActive(false);
        settingsObj.SetActive(true);
        eventSystem.SetSelectedGameObject(defaultSelectSettings);

        //set the sens slider to the correct spot based on the players sensitivity
        sensSlider.value = (playerSettings.GetSensitivity(playerInput.playerNum) / 300) * sensSlider.maxValue;
    }

    private void CloseSettingsMenu()
    {
        settingsObj.SetActive(false);
        OpenPauseMenu();
    }


}

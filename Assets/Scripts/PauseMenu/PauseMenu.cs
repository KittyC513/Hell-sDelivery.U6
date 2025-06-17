
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private GameObject menuObj;
    [SerializeField] private GameObject fadeObj;
    [SerializeField] private GameObject settingsObj;

    [SerializeField] private GameObject defaultSelectSettings;
    [SerializeField] private GameObject defaultSelectPause;

    private PauseManager pauseManager;

    private Vector2 menuInput;
    private Vector2 rawInput;

    private PlayerInputDetection playerInput;
    private bool playerSet = false;

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


    private void OpenPauseMenu()
    {
        fadeObj.SetActive(true);
        menuObj.SetActive(true);
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
    }

    private void CloseSettingsMenu()
    {
        settingsObj.SetActive(false);
        OpenPauseMenu();
    }


}

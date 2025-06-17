using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    //controls when the game is paused and unpaused
    //singleton 

    public static PauseManager Instance { get; private set; }

    //event that triggers when the game is paused
    public delegate void OnGamePause();
    public OnGamePause onGamePause;

    //event that triggers when the game is unpaused
    public delegate void OnGameResume();
    public OnGameResume onGameResume;

    public bool gamePaused = false;

    [HideInInspector] public PlayerInputDetection playerInControl;

    

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (gamePaused)
        {
            if (playerInControl.pausePressed)
            {
                UnpauseGame();
            }
        }
      
    }

    public void PauseGame(PlayerInputDetection player)
    {
        if (!gamePaused)
        {
            playerInControl = player;
            gamePaused = true;
            Time.timeScale = 0;
            onGamePause?.Invoke();
        }
    }

    public void UnpauseGame()
    {
        if (gamePaused)
        {
            gamePaused = false;
            Time.timeScale = 1;
            onGameResume?.Invoke();
        }
    }
}

using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    //event that triggers when the game is paused
    public delegate void OnGamePause();
    public OnGamePause onGamePause;

    //event that triggers when the game is unpaused
    public delegate void OnGameResume();
    public OnGameResume onGameResume;

    public bool gamePaused = false;

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
        if ( Input.GetKeyDown(KeyCode.P))
        {
            if (!gamePaused) PauseGame();
            else UnpauseGame();
        }
    }

    public void PauseGame()
    {
        if (!gamePaused)
        {
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

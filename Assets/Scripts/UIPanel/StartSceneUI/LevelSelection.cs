using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick_Minigame()
    {
        SceneManager.LoadScene("Minigame Testing Scene");
    }

    public void OnClick_Dialogue()
    {
        SceneManager.LoadScene("Dialogue_Testing");
    }

    public void OnClick_Tutorial()
    {
        SceneManager.LoadScene("Alleyway_tutorial_testing");
    }

    public void OnClick_Crane()
    {
        SceneManager.LoadScene("Crane_testing");
    }

    public void OnClick_BombDetonator()
    {
        SceneManager.LoadScene("BombDetonator_testing");
    }

    public void OnClick_Playtest()
    {
        SceneManager.LoadScene("Playtest-11-2025");
    }
}

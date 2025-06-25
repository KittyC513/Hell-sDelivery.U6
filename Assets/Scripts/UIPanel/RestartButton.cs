using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public CustomGUIButton restartButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        restartButton.clickEvent += () =>
        {
            SceneManager.LoadScene("Prototype Scene");
        };
    }


}

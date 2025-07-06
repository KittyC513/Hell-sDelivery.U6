using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DevPanel : MonoBehaviour
{
    public UnityEngine.UI.Toggle toggle_on;
    public UnityEngine.UI.Toggle toggle_off;

    public UnityEngine.UI.Toggle toggle_on_invisible;
    public UnityEngine.UI.Toggle toggle_off_invisible;


    public GameObject DevFunctionPanel;
    public Button btn_level1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toggle_on.onValueChanged.AddListener((b) =>
        {
            if (b)
            {
                DevFunctionPanel.SetActive(true);
            }
        });

        toggle_off.onValueChanged.AddListener((b) =>
        {
            if(b)
            {
                DevFunctionPanel.SetActive(false);
            }
        });

        btn_level1.onClick.AddListener(() =>
        {
            EventData.isAcceptedMission_lalah = true;
            SceneManager.LoadScene("Level1");
        });

        toggle_on_invisible.onValueChanged.AddListener((b) =>
        {
            if (b)
            {
                GameManager.instance.player1.layer = LayerMask.NameToLayer("Invisible_Player1");
                GameManager.instance.player2.layer = LayerMask.NameToLayer("Invisible_Player2");
            }
        });

        toggle_off_invisible.onValueChanged.AddListener((b) =>
        {
            if (b)
            {
                GameManager.instance.player1.layer = LayerMask.NameToLayer("Player1");
                GameManager.instance.player2.layer = LayerMask.NameToLayer("Player2");
            }
        });

        DevFunctionPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

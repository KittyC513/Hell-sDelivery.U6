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
        if (EventData.DevModeIsOn)
        {
            toggle_on.isOn = true;
            toggle_off.isOn = false;
            DevFunctionPanel.SetActive(true);
        }
        else
        {
            toggle_off.isOn = true;
            toggle_on.isOn = false;
            DevFunctionPanel.SetActive(false);
        }

        toggle_on.onValueChanged.AddListener((b) =>
        {
            if (b)
            {
                DevFunctionPanel.SetActive(true);
                EventData.DevModeIsOn = true;
            }
        });

        toggle_off.onValueChanged.AddListener((b) =>
        {
            if(b)
            {
                DevFunctionPanel.SetActive(false);
                EventData.DevModeIsOn = false;
            }
        });

        btn_level1.onClick.AddListener(() =>
        {
            EventData.isAcceptedMission_lalah = true;
            SceneManager.LoadScene("Level1");
        });

        if (toggle_off_invisible != null && toggle_on_invisible != null) 
        {
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

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

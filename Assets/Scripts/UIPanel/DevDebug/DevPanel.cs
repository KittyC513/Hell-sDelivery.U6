using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DevPanel : MonoBehaviour
{
    public UnityEngine.UI.Toggle toggle_on;
    public UnityEngine.UI.Toggle toggle_off;
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

        DevFunctionPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

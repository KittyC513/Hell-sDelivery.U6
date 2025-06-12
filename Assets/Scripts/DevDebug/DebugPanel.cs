using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugPanel : MonoBehaviour
{
    public CustomGUIButton btn_debug;
    public CustomGUIButton btn_ChangeScene;
    public CustomGUIButton btn_Alleyway;
    public CustomGUIButton btn_PostOffice;
    public CustomGUIButton btn_Level1;
    public CustomGUIButton btn_Exit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Turn on debug options when debug button is clicked
        btn_debug.clickEvent += () =>
        {
            print("Debug Options Enabled");
        };

        //Turn off debug options when exit button is clicked
        btn_Exit.clickEvent += () =>
        {
            print("Debug Options Disabled");
        };

        btn_ChangeScene.clickEvent += () =>
        {
            print("Scene Selection Enabled");
        };

        btn_Alleyway.clickEvent += () =>
        {
            //SceneManager.LoadScene("Alleyway");
            print("Loading Alleyway Scene");
        };

        btn_PostOffice.clickEvent += () =>
        {
            //SceneManager.LoadScene("PostOffice");
            print("Loading Post Office Scene");
        };  

        btn_Level1.clickEvent += () =>
        {
            //SceneManager.LoadScene("Level1");
            print("Loading Level 1 Scene");
        };

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

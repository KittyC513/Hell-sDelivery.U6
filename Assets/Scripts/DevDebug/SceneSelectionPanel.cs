using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelectionPanel : BasePanel<SceneSelectionPanel>
{
    public CustomGUIButton btn_alleyway;
    public CustomGUIButton btn_postOffice;
    public CustomGUIButton btn_level1;
    public CustomGUIButton btn_exit;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        btn_alleyway.clickEvent += () =>
        {
            print("Loading Alleyway Scene");
        };

        btn_postOffice.clickEvent += () =>
        {
            print("Loading Post Office Scene");
        };

        btn_level1.clickEvent += () =>
        {
            print("Loading Level 1 Scene");
        };

        btn_exit.clickEvent += () =>
        {
            HideMe();
            OptionPanel.Instance.HideMe();
        };

        HideMe();
    }

}

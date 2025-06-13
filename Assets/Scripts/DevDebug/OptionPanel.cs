using UnityEngine;

public class OptionPanel : BasePanel<OptionPanel>
{
    public CustomGUIButton btn_changeScene;
    public CustomGUIButton btn_invisible;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        btn_changeScene.clickEvent += () =>
        {
            SceneSelectionPanel.Instance.ShowMe();
            if (InvisiblePanel.Instance != null)
                InvisiblePanel.Instance.HideMe();
        };

        btn_invisible.clickEvent += () =>
        {
            if(SceneSelectionPanel.Instance != null)
                SceneSelectionPanel.Instance.HideMe();
            InvisiblePanel.Instance.ShowMe();
        };
        HideMe();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

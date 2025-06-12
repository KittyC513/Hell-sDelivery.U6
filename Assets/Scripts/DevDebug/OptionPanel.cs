using UnityEngine;

public class OptionPanel : BasePanel<OptionPanel>
{
    public CustomGUIButton btn_changeScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        btn_changeScene.clickEvent += () =>
        {
            SceneSelectionPanel.Instance.ShowMe();
        };
        HideMe();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

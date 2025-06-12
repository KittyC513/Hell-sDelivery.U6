using UnityEngine;

public class DebugbtnControl : MonoBehaviour
{
    public CustomGUIButton btn_Debug;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        btn_Debug.clickEvent += () =>
        {
            OptionPanel.Instance.ShowMe();
            print("Debug Button Clicked");
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

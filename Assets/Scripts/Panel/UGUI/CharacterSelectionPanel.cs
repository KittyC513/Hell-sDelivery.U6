using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionPanel : MonoBehaviour
{
    [SerializeField]
    private Text p1Select_leftScreen;
    [SerializeField]
    private Text p2Select_leftScreen;
    [SerializeField]
    private Text p1Select_rightScreen;
    [SerializeField]
    private Text p2Select_rightScreen;

    public bool p1Selected_leftScreen = false;
    public bool p2Selected_leftScreen = false;
    public bool p1Selected_rightScreen = false;
    public bool p2Selected_rightScreen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        p1Select_leftScreen.enabled = false;
        p2Select_leftScreen.enabled = false;
        p1Select_rightScreen.enabled = false;
        p2Select_rightScreen.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        SetPlayerSelection();
    }


    void SetPlayerSelection()
    {
        if(p1Selected_leftScreen)
        {
            p1Select_leftScreen.enabled = true;
        }
        else
        {
            p1Select_leftScreen.enabled = false;
        }
        
        if (p2Selected_leftScreen)
        {
            p2Select_leftScreen.enabled = true;
        }
        else
        {
            p2Select_leftScreen.enabled = false;
        }
        
        if (p1Selected_rightScreen)
        {
            p1Select_rightScreen.enabled = true;
        }
        else
        {
            p1Select_rightScreen.enabled = false;
        }
        
        if (p2Selected_rightScreen)
        {
            p2Select_rightScreen.enabled = true;
        }
        else
        {
            p2Select_rightScreen.enabled = false;
        }

    }

}

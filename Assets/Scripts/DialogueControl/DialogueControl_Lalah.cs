using PixelCrushers.DialogueSystem;
using UnityEngine;

public class DialogueControl_Lalah : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (EvenData.isAcceptedMission_lalah && this.GetComponent<Usable>().enabled)
        {
            this.GetComponent<Usable>().enabled = false;
        }
    }
}

using UnityEngine;

public class BarkTriggerControl : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneControl_Level_greyBox.Instance.SwapBarkConversation("Devil/DevilIntro");
            SceneControl_Level_greyBox.Instance.EnableDevilDialogue();
        }
    }
}

using System.Collections;
using UnityEngine;

public class BarkTriggerControl : MonoBehaviour
{
    public float delayTime = 3f;
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
            print("Player entered bark trigger area");
            SceneControl_Level_greyBox.Instance.SwapBarkConversation("Devil Barks/Tresspass");
            SceneControl_Level_greyBox.Instance.EnableDevilDialogue();
            StartCoroutine(DisableDialogueTrigger(delayTime));
        }
    }


    IEnumerator DisableDialogueTrigger(float timer)
    {
        print("Coroutine Starts");
        yield return new WaitForSeconds(timer);
        SceneControl_Level_greyBox.Instance.DisableDevilDialogue();
    }
}

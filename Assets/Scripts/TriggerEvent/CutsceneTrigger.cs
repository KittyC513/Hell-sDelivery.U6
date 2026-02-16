using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
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
            // Trigger the cutscene here
            SceneControl_MainMenu.Instance.cutscene_tutorialEnd.SetActive(true);
            print("Cutscene triggered!");
        }
    }
}

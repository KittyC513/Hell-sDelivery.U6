using System.Collections;
using UnityEngine;

public class CutsceneTrigger_greyBoxLevel : MonoBehaviour
{
    public bool isCutSceneTriggered = false;
    public GameObject cutSceneObj;
    public float duration;
    public GameObject canvas_fadeIn;

    public Transform[] spawnpoints;
    public bool useCollider = true;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerCutscene()
    {
        print("Cutscene starts!");

        cutSceneObj.SetActive(true);
        EventData.isOnCutScene = true;
        StartCoroutine(OnCutSceneEnd(duration));
        print("Cutscene triggered!");
        isCutSceneTriggered = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (useCollider)
        {
            if (cutSceneObj == null)
            {
                print("Cutscene object is not assigned");
                return;
            }

            if (isCutSceneTriggered)
            {
                print("Cutscene triggered!");
                return;
            }
                

            if (other.CompareTag("Player"))
            {
                TriggerCutscene();
            }
        }
       
    }

    IEnumerator OnCutSceneEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        EventData.isOnCutScene = false;
        cutSceneObj.SetActive(false);
        yield return new WaitForSeconds(1f);
        canvas_fadeIn.SetActive(false);
        GameManager.Instance.ResetPlayersPosition(spawnpoints[0], spawnpoints[1]);
    }


}

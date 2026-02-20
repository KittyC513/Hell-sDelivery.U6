using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger_greyBoxLevel : MonoBehaviour
{
    public bool isCutSceneTriggered = false;
    public GameObject cutSceneObj;
    public float duration;
    public GameObject canvas_fadeIn;

    public Transform[] spawnpoints;
    public bool useCollider = true;
    private PlayableDirector cutsceneDirector;

    public GameObject animationPlayer_01;
    public GameObject animationPlayer_02;



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
        animationPlayer_01.gameObject.SetActive(true);
        animationPlayer_02.gameObject.SetActive(true);
        GameManager.Instance.ResetPlayersPosition(spawnpoints[2], spawnpoints[3]);
        GameManager.Instance.FreezeBothPlayers();
        print("Cutscene starts!");

        cutSceneObj.SetActive(true);
        EventData.isOnCutScene = true;
        StartCoroutine(OnCutSceneEnd(duration));
        print("Cutscene triggered!");
        isCutSceneTriggered = true;
    }

    private void OnTriggerEnter(Collider other)
    {



        if (useCollider && !isCutSceneTriggered)
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

    public void ForceStopCutscene()
    {
        StopAllCoroutines();
        canvas_fadeIn.SetActive(false);
        GameManager.Instance.ResetPlayersPosition(spawnpoints[0], spawnpoints[1]);
        Debug.Log("Cutscene Ended");
    }



    IEnumerator OnCutSceneEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        animationPlayer_01.SetActive(false);
        animationPlayer_02.SetActive(false);
        cutSceneObj.SetActive(false);
        yield return new WaitForSeconds(1f);
        canvas_fadeIn.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        EventData.isOnCutScene = false;
        GameManager.Instance.ResetPlayersPosition(spawnpoints[0], spawnpoints[1]);
        GameManager.Instance.UnFreezeBothPlayers();
        Debug.Log("Cutscene Ended");

    }

}

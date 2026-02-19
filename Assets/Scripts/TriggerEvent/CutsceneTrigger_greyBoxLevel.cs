using System.Collections;
using UnityEngine;

public class CutsceneTrigger_greyBoxLevel : MonoBehaviour
{
    public GameObject cutSceneObj;
    public float duration;
    public GameObject canvas_fadeIn;

    public Transform[] spawnpoints;
    

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
        if (cutSceneObj == null) return;

        if(other.CompareTag("Player"))
        {
            cutSceneObj.SetActive(true);
            EventData.isOnCutScene = true;
            StartCoroutine(OnCutSceneEnd(duration));
            print("Cutscene triggered!");
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
        this.enabled = false;
    }


}

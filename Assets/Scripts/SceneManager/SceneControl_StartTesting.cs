using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl_StartTesting : MonoBehaviour
{
    public PlayerManager playerManager;
    public GameObject levelSelection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventData.curSceneName = "StartTesting";
    }

    // Update is called once per frame
    void Update()
    {
        if(playerManager.players.Count == 2)
        {
            GameManager.instance.cam_p1.rect = new Rect(0, 0, 0.5f, 1);
            GameManager.instance.cam_p2.rect = new Rect(0.5f, 0, 0.5f, 1);
            levelSelection.SetActive(true);
        }
    }
}

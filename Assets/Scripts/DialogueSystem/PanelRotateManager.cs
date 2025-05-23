using UnityEngine;
using UnityEngine.UI;

public class PanelRotateManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject panel_p1;
    public GameObject panel_p2;

    public PlayerManager playerManager;
    private Transform p1Cam;
    private Transform p2Cam;

    [Header("UI info")]
    public Text text_p1;
    public Text text_p2;

    private void OnEnable()
    {
        //playerManager = FindFirstObjectByType<PlayerManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if(playerManager.players.Count == 1)
        {
            p1Cam = playerManager.players[0].gameObject.GetComponent<PlayerInputDetection>().cam.transform;

        }

        if (playerManager.players.Count == 2)
        {
            p2Cam = playerManager.players[1].gameObject.GetComponent<PlayerInputDetection>().cam.transform;
        }

        FollowP1Rotate();
        FollowP2Rotate();
    }

    void FollowP1Rotate()
    {
        if (p1Cam != null)
        {
            panel_p1.transform.LookAt(panel_p1.transform.position + p1Cam.rotation * Vector3.forward, p1Cam.rotation * Vector3.up);
        }

    }

    void FollowP2Rotate()
    {
        if(p2Cam != null)
        {
            panel_p2.transform.LookAt(panel_p2.transform.position + p2Cam.rotation * Vector3.forward, p2Cam.rotation * Vector3.up);

        }

        if(text_p1 != null)
        {
            text_p2.text = text_p1.text;
        }


    }
}

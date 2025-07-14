using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class BillboardUI : MonoBehaviour
{
    private Camera p1Cam;
    private Camera p2Cam;

    [SerializeField] private Canvas billboardPrefab;
    [SerializeField] private Transform positionFollow;

    [SerializeField] private bool player1Active = false;
    [SerializeField] private bool player2Active = false;

    private List<GameObject> images;


    private void Start()
    {
        p1Cam = GameManager.instance.cam_p1;
        p2Cam = GameManager.instance.cam_p2;
        images = new List<GameObject>();

        images.Add(Instantiate(billboardPrefab, this.transform.position, Quaternion.identity, this.transform).gameObject);
        images.Add(Instantiate(billboardPrefab, this.transform.position, Quaternion.identity, this.transform).gameObject);

        //0 = player 1
        //1 = player 2
        images[0].layer = LayerMask.NameToLayer("UI_P2Ignore");
        images[1].layer = LayerMask.NameToLayer("UI_P1Ignore");
    }

    public void ShowIconToPlayer(bool show, int player)
    {
        if (player == 1)
        {
            player1Active = show;
        }
        else
        {
            player2Active = show;   
        }
    }

    private void Update()
    {
        //set the camera if its still null
        if (p1Cam == null) p1Cam = GameManager.instance.cam_p1;
        if (p2Cam == null) p2Cam = GameManager.instance.cam_p2;

        //show the ui if the player is in range
        if (player1Active && p1Cam != null)
        {
            images[0].SetActive(true);
            BillboardToCamera(p1Cam, images[0]);
        }
        else
        {
            images[0].SetActive(false);
        }

        if (player2Active && p2Cam != null)
        {
            images[1].SetActive(true);
            BillboardToCamera(p2Cam, images[1]);
        }
        else
        {
            images[1].SetActive(false);
        }
    }

    private void BillboardToCamera(Camera cam, GameObject img)
    {
        img.transform.position = positionFollow.position;

        img.transform.LookAt(cam.transform.position);

    }
}


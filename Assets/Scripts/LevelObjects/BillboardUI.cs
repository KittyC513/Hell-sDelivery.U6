using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class BillboardUI : MonoBehaviour
{
    private Camera p1Cam;
    private Camera p2Cam;

    [SerializeField] private Canvas billboardPrefab;
    //[SerializeField] private Transform positionFollow;

    [SerializeField] private bool player1Active = false;
    [SerializeField] private bool player2Active = false;

    [SerializeField] private bool globalIcons = false;

    [SerializeField] private float yOffset = 0.5f;

    [SerializeField] private bool applySine = true;
    [SerializeField] private float frequency = 2.5f;
    [SerializeField] private float amplitude = 0.15f;

    private Vector3 pos;
    private Vector3 startPos;
    private Vector3 changeInPos;

    private List<GameObject> images;
    public bool active = true;


    private void Start()
    {
        
        //change this to grab an instance from the active player to get their current camera
        //rather than the player camera so that it can be used with different camera types such as
        //the minigame camera
        p1Cam = GameManager.instance.cam_p1;
        p2Cam = GameManager.instance.cam_p2;
        images = new List<GameObject>();

        images.Add(Instantiate(billboardPrefab, this.transform.position, Quaternion.identity, this.transform).gameObject);
        images.Add(Instantiate(billboardPrefab, this.transform.position, Quaternion.identity, this.transform).gameObject);

        //0 = player 1
        //1 = player 2

        if (!globalIcons)
        {
            images[0].layer = LayerMask.NameToLayer("UI_P2Ignore");
            images[1].layer = LayerMask.NameToLayer("UI_P1Ignore");
        }
        else
        {
            images[0].GetComponentInChildren<Image>().color = Color.red;
            images[1].GetComponentInChildren<Image>().color = Color.blue;
        }

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

    public bool IsShowingToPlayer(int player)
    {
        if (player == 1)
        {
            return player1Active;
        }
        else
        {
            return player2Active;
        }
    }

    private void Update()
    {
        //update the unaltered position
        startPos = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);

        //set the camera if its still null
        if (p1Cam == null) p1Cam = GameManager.instance.cam_p1;
        if (p2Cam == null) p2Cam = GameManager.instance.cam_p2;

        //show the ui if the player is in range
        if (player1Active && p1Cam != null && active)
        {
            images[0].SetActive(true);
            BillboardToCamera(p1Cam, images[0]);
        }
        else
        {
            images[0].SetActive(false);
        }

        if (player2Active && p2Cam != null && active)
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
        //apply the y offset from the sin wave
        float yPos = amplitude * Mathf.Sin(Time.time * frequency);
        pos = new Vector3(startPos.x, startPos.y + yPos, startPos.z);

        img.transform.position = pos;

        img.transform.LookAt(cam.transform.position);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z), new Vector3(0.25f, 0.25f + amplitude, 0.25f));
    }

}


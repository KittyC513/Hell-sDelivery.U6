using System.Collections;
using UnityEngine;

public class EnterCrane : MonoBehaviour
{
    public bool p1EnterCrane = false;
    public bool p2EnterCrane = false;

    public Transform pos_controlRoom;
    public Camera cam_crane;
    public Camera cam_pos;
    private InteractableObject interactable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam_crane.enabled = false;
        cam_pos.enabled = false;
        p1EnterCrane = false; 
        p2EnterCrane = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!EventData.craneIsActivated)
        {
            cam_pos.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (EventData.craneIsActivated) return;
        return;
        //Crane Activation
        cam_crane.enabled = true;
        if (other.CompareTag("Player"))
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Player1") || other.gameObject.layer == LayerMask.NameToLayer("Invisible_Player1"))
            {
                //reset player position to control room
                GameManager.instance.ResetPlayer1Position(pos_controlRoom);
                //freeze player movement
                GameManager.instance.FreezePlayer1();
                //disable player cam
                GameManager.instance.DisablePlayer1Cam();

                //set cam rect
                cam_crane.rect = new Rect(0, 0, 0.5f, 1);
                p1EnterCrane = true;
                print("Player 1 Enter Crane");
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Player2") || other.gameObject.layer == LayerMask.NameToLayer("Invisible_Player2"))
            {
                //reset player position to control room
                GameManager.instance.ResetPlayer2Position(pos_controlRoom);
                //freeze player movement
                GameManager.instance.FreezePlayer2();
                //disable player cam
                GameManager.instance.DisablePlayer2Cam();


                //set cam rect
                cam_crane.rect = new Rect(0.5f, 0, 0.5f, 1);
                p2EnterCrane = true;
            }

            EventData.craneIsActivated = true;
            print("Crane is Activated");
        }
    }

    public void PlayerEnterCrane(PlayerInputDetection player, InteractableObject _interactable)
    {
        if (EventData.craneIsActivated) return;

        int playerNum = player.playerNum;

        interactable = _interactable;
        interactable.canInteract = false;


        switch (playerNum)
        {
            case 1:

                //reset player position to control room
                GameManager.instance.ResetPlayer1Position(pos_controlRoom);
                //freeze player movement
                GameManager.instance.FreezePlayer1();
                //disable player cam
                GameManager.instance.DisablePlayer1Cam();

                //set cam rect
                cam_crane.rect = new Rect(0, 0, 0.5f, 1);
                p1EnterCrane = true;
                print("Player 1 Enter Crane");


            break;
            case 2:

                //reset player position to control room
                GameManager.instance.ResetPlayer2Position(pos_controlRoom);
                //freeze player movement
                GameManager.instance.FreezePlayer2();
                //disable player cam
                GameManager.instance.DisablePlayer2Cam();


                //set cam rect
                cam_crane.rect = new Rect(0.5f, 0, 0.5f, 1);
                p2EnterCrane = true;

            break;
        }

        StartCoroutine(EnterDelay());
    }

    private IEnumerator EnterDelay()
    {
        yield return new WaitForSeconds(0.025f);

        cam_crane.enabled = true;
        EventData.craneIsActivated = true;
    }

    //called when the crane is exited
    public void ResetEntrance()
    {
        GameManager.instance.UnFreezeBothPlayers();
        GameManager.instance.EnableBothPlayersCam();
        p1EnterCrane = false;
        p2EnterCrane = false;
        EventData.craneIsActivated = false;
        cam_crane.enabled = false;

        StartCoroutine(ExitDelay());
    }

    //called because theres a bug when exiting the crane and entering right after that freezes the camera
    private IEnumerator ExitDelay()
    {
        yield return new WaitForSeconds(0.5f);

        interactable.canInteract = true;
    }




}

using UnityEngine;
using UnityEngine.InputSystem;

public class SceneControl_Level1 : SceneControlBase
{
    public Transform[] enterPoints;
    public EnterCrane enterCrane;

    public Transform crane;
    private PlayerInputDetection playerInput;

    public float rotationSpeed = 30;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ResetPlayerPos();
        EventData.curSceneName = "Level1";
        EventData.craneIsActivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        EnterCraneScriptControl();
        ResetPlayerPos();
    }

    public void ResetPlayerPos()
    {
        if (!isResetPos)
        {
            GameManager.instance.player1.transform.position = enterPoints[0].position;
            GameManager.instance.player2.transform.position = enterPoints[1].position;
            isResetPos = true;
        }
    }

    public void EnterCraneScriptControl()
    {
        if (EventData.craneIsActivated && enterCrane.GetComponent<Collider>().enabled)
        {
            //enterCrane.GetComponent<Collider>().enabled = false;
        }
        else if (!EventData.craneIsActivated && !enterCrane.GetComponent<Collider>().enabled)
        {
            //enterCrane.GetComponent<Collider>().enabled = true;
        }

        CraneMovement();
    }

    public void CraneMovement()
    {

        if(enterCrane.p1EnterCrane)
        {
            GameManager.instance.FreezePlayer1();
            playerInput = GameManager.instance.player1.GetComponent<PlayerInputDetection>();

            // Calculate rotation based on horizontal input
            float inputX = playerInput.GetHorizontalMovement().x;
            crane.Rotate(Vector3.up, inputX * rotationSpeed * Time.deltaTime);
        }
        else
        {
            GameManager.instance.UnFreezePlayer1();
        }

        if (enterCrane.p2EnterCrane)
        {
            GameManager.instance.FreezePlayer2();
            playerInput = GameManager.instance.player2.GetComponent<PlayerInputDetection>();
            // Calculate rotation based on horizontal input
            float inputX = playerInput.GetHorizontalMovement().x;
            crane.Rotate(Vector3.up, inputX * rotationSpeed * Time.deltaTime);
        }
        else
        {
            GameManager.instance.UnFreezePlayer2();
        }

    }


}

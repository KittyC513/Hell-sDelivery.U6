using UnityEngine;

public enum E_CamType
{
    playerCam,
    lockCam,
}
public class CameraManager : MonoBehaviour
{
    public PlayerInputDetection inputDetection;

    public Camera playerCam;
    public Camera lockCam;

    public E_CamType currentCamType;
    public PlayerLockOn playerLockOn;

    public CameraMovement_Lock cameraMovement_Lock;
    public CameraMovement_Player cameraMovement_Player;

    public Transform defaultPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        
    }
    void Start()
    {
        AddCullingMaskOnPlayers();
    }

    // Update is called once per frame
    void Update()
    {

        ////Detect if there's target to lock on to
        //if (playerLockOn.lockTarget != null)
        //{
        //    currentCamType = E_CamType.lockCam;
        //}
        //else
        //{
        //    Debug.Log("No target to lock on to");
        //}

        
        //method to switch between cameras
        switch (currentCamType)
        {
            case E_CamType.playerCam:
                SwitchToPlayerCam();
                break;
            case E_CamType.lockCam:
                SwitchToLockCam();
                break;
        }
    }

    public void ResetCamTransition()
    {
       
        switch (currentCamType)
        {
            case E_CamType.playerCam:

                break;
            case E_CamType.lockCam:
                lockCam.transform.position = playerCam.transform.position;
                break;
        }
    }

    public void SwitchToPlayerCam()
    {
        lockCam.gameObject.SetActive(false);
        playerCam.gameObject.SetActive(true);
        inputDetection.cam = playerCam;
    }

    public void SwitchToLockCam()
    {
        playerCam.gameObject.SetActive(false);
        lockCam.gameObject.SetActive(true);
        inputDetection.cam = lockCam;
    }

    //Add culling mask according to player1 and player2
    //if it's player1, don't render p2UI, and vice verse
    void AddCullingMaskOnPlayers()
    {
        if(inputDetection.gameObject.layer == LayerMask.NameToLayer("Player1"))
        {
            playerCam.cullingMask &= ~(1 << LayerMask.NameToLayer("UI_P1Ignore"));
            lockCam.cullingMask &= ~(1 << LayerMask.NameToLayer("UI_P1Ignore"));
        }

        if (inputDetection.gameObject.layer == LayerMask.NameToLayer("Player2"))
        {
            playerCam.cullingMask &= ~(1 << LayerMask.NameToLayer("UI_P2Ignore"));
            lockCam.cullingMask &= ~(1 << LayerMask.NameToLayer("UI_P2Ignore"));
        }
    }
}

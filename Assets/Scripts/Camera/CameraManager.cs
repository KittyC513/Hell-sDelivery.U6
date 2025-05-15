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
                cameraMovement_Player.ResetPos();
                break;
            case E_CamType.lockCam:
                cameraMovement_Player.resetPos = false;
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
}

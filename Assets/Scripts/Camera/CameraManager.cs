using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum E_CamType
{
    playerCam,
    lockCam,
    mainCam,
}
public class CameraManager : MonoBehaviour
{
    public PlayerInputDetection inputDetection;

    public Camera playerCam;
    public Camera lockCam;
    public Camera mainCam;

    #region Alleyway Scene Cam
    public Camera alleywayCam;
    #endregion

    public E_CamType currentCamType = E_CamType.playerCam;
    public PlayerLockOn playerLockOn;

    public CameraMovement_Lock cameraMovement_Lock;
    public CameraMovement_Player cameraMovement_Player;

    public Transform defaultPos;
    
    private bool isResetCam = false;



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
        DetectScene();

        //method to switch between cameras
        switch (currentCamType)
        {
            case E_CamType.playerCam:
                SwitchToPlayerCam();
                break;
            case E_CamType.lockCam:
                SwitchToLockCam();
                break;
            case E_CamType.mainCam:
                SwitchToMainCam();              
                
                #region Alleyway Scene Cam - comment out for now
                //SwitchToAlleywayCam();
                #endregion
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
        if (EventData.craneIsActivated) return;
        lockCam.enabled = false;
        playerCam.enabled = true;
        inputDetection.cam = playerCam;
    }

    public void SwitchToLockCam()
    {
        playerCam.enabled = false;
        lockCam.enabled = true;
        inputDetection.cam = lockCam;
    }
    public void SwitchToMainCam()
    {
        if(mainCam == null || EventData.isSceneChanged)
        {
            mainCam = Camera.main;
            EventData.isSceneChanged = false;
        }

        playerCam.enabled = false;
        lockCam.enabled = false;
        inputDetection.cam = mainCam;
    }

    #region Alleyway Scene Cam
    public void SwitchToAlleywayCam()
    {
        playerCam.enabled = false;
        lockCam.enabled = false;
        alleywayCam.enabled = true;
        inputDetection.cam = alleywayCam;

    }
    #endregion

    //Add culling mask according to player1 and player2
    //if it's player1, don't render p2UI, and vice verse
    void AddCullingMaskOnPlayers()
    {
        if(inputDetection.gameObject.layer == LayerMask.NameToLayer("Player1") || 
            inputDetection.gameObject.layer == LayerMask.NameToLayer("Invisible_Player1"))
        {
            playerCam.cullingMask &= ~(1 << LayerMask.NameToLayer("UI_P1Ignore"));
            lockCam.cullingMask &= ~(1 << LayerMask.NameToLayer("UI_P1Ignore"));
        }

        if (inputDetection.gameObject.layer == LayerMask.NameToLayer("Player2") || 
            inputDetection.gameObject.layer == LayerMask.NameToLayer("Invisible_Player2"))
        {
            playerCam.cullingMask &= ~(1 << LayerMask.NameToLayer("UI_P2Ignore"));
            lockCam.cullingMask &= ~(1 << LayerMask.NameToLayer("UI_P2Ignore"));
        }
    }


    #region Detect Scene

    public void DetectScene()
    {

        if (EventData.curSceneName == "PostOffice" || 
            EventData.curSceneName == "StartScene" || 
            EventData.curSceneName == "minigame" ||
            EventData.curSceneName == "StartTesting")
        {
            currentCamType = E_CamType.mainCam;
        }

    }

    #endregion


}

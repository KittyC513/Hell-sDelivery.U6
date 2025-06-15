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
                //SwitchToMainCam();
                #region Alleyway Scene Cam
                SwitchToAlleywayCam();
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
    public void SwitchToMainCam()
    {
        if(mainCam == null)
        {
            mainCam = Camera.main;

        }

        playerCam.gameObject.SetActive(false);
        lockCam.gameObject.SetActive(false);
        inputDetection.cam = mainCam;

        //Assign pos on main camera for player1 and player2
        if (inputDetection.gameObject.layer == LayerMask.NameToLayer("Player1"))
        {
            mainCam.GetComponent<CameraMovement_Scene>().p1Pos = inputDetection.transform;
        }

        if (inputDetection.gameObject.layer == LayerMask.NameToLayer("Player2"))
        {
            mainCam.GetComponent<CameraMovement_Scene>().p2Pos = inputDetection.transform;
        }
    }

    #region Alleyway Scene Cam
    public void SwitchToAlleywayCam()
    {
        playerCam.gameObject.SetActive(false);
        lockCam.gameObject.SetActive(false);
        alleywayCam.gameObject.SetActive(true);
        inputDetection.cam = alleywayCam;

    }
    #endregion

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


    #region Detect Scene

    public void DetectScene()
    {
        if(SceneManager.GetActiveScene().name == "Alleyway")
        {
            currentCamType = E_CamType.mainCam;
        }
    }

    #endregion


}

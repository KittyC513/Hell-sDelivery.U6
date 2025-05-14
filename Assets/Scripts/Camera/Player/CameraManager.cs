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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentCamType = E_CamType.lockCam;
        }
        switch (currentCamType)
        {
            case E_CamType.playerCam:
                playerCam.gameObject.SetActive(true);
                lockCam.gameObject.SetActive(false);
                inputDetection.cam = playerCam;

                break;
            case E_CamType.lockCam:
                playerCam.gameObject.SetActive(false);
                lockCam.gameObject.SetActive(true);
                inputDetection.cam = lockCam;
                break;
        }
    }
}

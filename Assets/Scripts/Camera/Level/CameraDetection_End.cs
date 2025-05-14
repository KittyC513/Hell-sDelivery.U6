using UnityEngine;

public class CameraDetection_End : MonoBehaviour
{
    public Camera cam;
    public bool switchCam = false;
    
    [HideInInspector]
    public bool camSwiched = false;

    private PlayerInputDetection playerInputDetection;
    
    [HideInInspector]
    public Camera playerCam;

    public CameraDetection_Start CameraDetection_Start;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(playerCam == null)
                playerCam = other.GetComponent<PlayerInputDetection>().playerCam.gameObject.GetComponent<Camera>();
            Debug.Log("Player cam: " + playerCam);

            switchCam = true;
            playerInputDetection = other.GetComponent<PlayerInputDetection>();
        }
    }

    void LateUpdate()
    {
        if (switchCam && !camSwiched)
            SwitchCamera();
    }

    public void SwitchCamera()
    {

        playerCam.enabled = true;
        playerInputDetection.cam = playerCam;
        camSwiched = true;
        CameraDetection_Start.camSwiched = false;
        CameraDetection_Start.switchCam = false;
        Debug.Log("End Switch function");
    }
}

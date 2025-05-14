using PixelCrushers.DialogueSystem.SequencerCommands;
using UnityEngine;

public class CameraDetection_Start : MonoBehaviour
{
    public Camera cam;
    public bool switchCam = false;
    [HideInInspector]
    public bool camSwiched = false;

    private PlayerInputDetection playerInputDetection;

    public CameraDetection_End cameraDetection_End;

    public CameraMovement_Level cameraMovement_Level;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switchCam = true;
            playerInputDetection = other.GetComponent<PlayerInputDetection>();
            
        }
    }

    void LateUpdate()
    {
        if (switchCam && !camSwiched)
            SwitchCamera();
        if (camSwiched)
            cameraMovement_Level.FollowPlayerMovement(playerInputDetection.gameObject.transform);
        else
            cameraMovement_Level.transform.position = cam.transform.position;

    }

    public void SwitchCamera()
    {

        playerInputDetection.cam.gameObject.GetComponent<Camera>().enabled = false;
        playerInputDetection.cam = cam;
        camSwiched = true;
        cameraDetection_End.camSwiched = false;
        cameraDetection_End.switchCam = false;
        Debug.Log("Start Switch function");
    }

}

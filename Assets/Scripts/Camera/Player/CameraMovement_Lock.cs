using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum E_LockCamType
{
    one,
    two,
    three,
}
public class CameraMovement_Lock : MonoBehaviour
{
    public Transform player;
    private Vector3 afterPos;
    public Vector3 offSet;

    public float smoothSpeed = 5f;
    public E_LockCamType lockCamType;

    public PlayerLockOn playerLockOn;

    public float distance = 5f; // Distance from the target
    public float height = 2f; // Height above the target

   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lockCamType = E_LockCamType.three;
    }
    private void Update()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        switch(lockCamType)
        {
            case E_LockCamType.one:
                CameraMovementMethod1();
                break;
            case E_LockCamType.two:
                CameraMovementMethod2();
                break;
            case E_LockCamType.three:
                CameraMovementMethod3();
                break;
        }

    }

    #region camera movement Method 1 - only follow player.x and z
    private void CameraMovementMethod1()
    {
       

        afterPos = player.position + offSet;
        this.transform.position = afterPos;
        this.transform.LookAt(player, Vector3.up);
    }
    #endregion

    #region camera movement Method 2 - Always behind player
    private void CameraMovementMethod2()
    {
        

        //Rotate the offset to always be behind the player
        Vector3 desiredPosition = player.position + player.rotation * offSet;

        //Smooth camera movement
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

        //Look at the player
        transform.LookAt(player.position + Vector3.up * 1.5f); // Slightly above the player’s center
    }
    #endregion

    #region camera movement Method 3 - Centered between Player and Target

    private void CameraMovementMethod3()
    {
        if(playerLockOn.lockTarget != null)
        {
            // Calculate the midpoint between the player and the target
            Vector3 midpoint = (player.position + playerLockOn.lockTarget.transform.position) / 2;
            // Direction from target to player
            Vector3 dir = (player.position - playerLockOn.lockTarget.transform.position).normalized;
            // Camera offset
            Vector3 desiredPosition = midpoint + dir * distance + Vector3.up * height;
            // Smooth camera movement
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
            // Look at the target (or midpoint)
            transform.LookAt(playerLockOn.lockTarget.transform.position + Vector3.up * 1.5f);
            // Look at the midpoint
            //transform.LookAt(midpoint);
        }
        

    }
    #endregion

    
}

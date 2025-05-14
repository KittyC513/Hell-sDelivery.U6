using UnityEngine;

public class CameraMovement_Lock : MonoBehaviour
{
    public Transform player;
    private Vector3 afterPos;
    public Vector3 offSet;

    public float smoothSpeed = 5f;
    public bool isMethod1 = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void Update()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        #region camera movement Method 1 - only follow player.x and z

        afterPos = player.position + offSet;
        if (isMethod1)
        {
            this.transform.position = afterPos;
            this.transform.LookAt(player, Vector3.up);
        }
        #endregion
        else
        {
            #region camera movement Method 2 - Always behind player
            //Rotate the offset to always be behind the player
            Vector3 desiredPosition = player.position + player.rotation * offSet;

            //Smooth camera movement
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

            //Look at the player
            transform.LookAt(player.position + Vector3.up * 1.5f); // Slightly above the player’s center
            #endregion
        }




    }
}

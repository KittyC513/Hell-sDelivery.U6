using UnityEngine;

public class CameraFollowFunction : MonoBehaviour
{
    public Transform player;
    public Vector3 offSet;

    public float smoothSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate the offset to always be behind the player
        Vector3 desiredPosition = player.position + player.rotation * offSet;

        //Smooth camera movement
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

        //Look at the player
        transform.LookAt(player.position + Vector3.up * 1.5f); // Slightly above the player’s center
    }
}

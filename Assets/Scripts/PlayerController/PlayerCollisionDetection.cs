using UnityEngine;

public class PlayerCollisionDetection : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("EditorOnly"))
        {
            Debug.Log(this.gameObject.layer.ToString() + "Player collided with an EditorOnly!");

        }

    }
}

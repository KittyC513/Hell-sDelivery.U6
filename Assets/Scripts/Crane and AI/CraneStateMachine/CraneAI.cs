using UnityEngine;
using UnityEngine.AI;

public class CraneAI : MonoBehaviour
{
    Animator anim;
    public Transform player;
    CraneState currentState;
    public Transform pos_pickupLocation;
    public Transform pos_dropoffLocation;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(player == null)
        {
            print("Player is not assigned in Insepctor");
            //player = GameManager.instance.player1.transform;
        }
        Vector3 pickupLocation = pos_pickupLocation.position;
        Vector3 dropoffLocation = pos_dropoffLocation.position;

        print("Pickup Location: " + pickupLocation);
        print("Dropoff Location: " + dropoffLocation);

        anim = this.GetComponent<Animator>();
        currentState = new Idle(this.gameObject, anim, player, pickupLocation, dropoffLocation);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == null)
        {
            print("Current State is null");
            return;
        }

        CraneState next = currentState.Process();

        if (next != null)
            currentState = next;
        else
            Debug.Log("Current State returned null");

    }
}

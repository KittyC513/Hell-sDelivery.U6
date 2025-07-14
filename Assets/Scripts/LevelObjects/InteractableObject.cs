using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    //range you can interact with this object from
    [SerializeField] private float interactRange;

    //UI that displays when you can interact with this object
    [SerializeField] private BillboardUI interactUI;

    //trigger event
    [SerializeField] public UnityEvent OnInteract;

    [SerializeField] private bool limitInteracts = true;
    [SerializeField] private int maxNumberOfInteracts = 1;
    private bool canInteract = true;

    private int interactCount = 0;

    [SerializeField] private Collider[] players;
    private Collider[] oldPlayers;
    private PlayerInputDetection[] pInput;
    private bool colliding = false;

    [SerializeField] private LayerMask playerMask;

    private void Start()
    {
        //initialize the array that holds players in range
        players = new Collider[2];
        oldPlayers = new Collider[2];
        pInput = new PlayerInputDetection[2];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }

    private void Update()
    {
        if (colliding && canInteract)
        {
            for (int i = 0; i < pInput.Length; i++)
            {
                if (pInput[i] != null)
                {
                    //a player has pressed the crouch input
                    if (pInput[i].interactPressed)
                    {
                        //interact with this object and pass in the player object
                        OnInteract.Invoke();
                        interactCount++;
                    }
                }
            }
        }

        if (limitInteracts)
        {
            if (interactCount >= maxNumberOfInteracts && canInteract == true)
            {
                canInteract = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (canInteract)
        {
            DetectPlayers();
        }
        else
        {
            //stop showing the UI if the object can no longer be interacted with
            interactUI.ShowIconToPlayer(false, 1);
            interactUI.ShowIconToPlayer(false, 2);
        }
    }

    private void DetectPlayers()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, interactRange, players, playerMask);

        //check if any collisions are happening
        if (numColliders > 0)
        {
            colliding = true;
        }
        else
        {
            colliding = false;
        }

        //numColliders is how many players are colliding
        //0 is always going to be the first player to collide
        for (int i = 0; i < numColliders; i++)
        {
            //set the player input
            pInput[i] = players[i].GetComponentInParent<PlayerInputDetection>();

            //check which player is colliding and show ui to them
            interactUI.ShowIconToPlayer(true, pInput[i].playerNum);
        }

        for (int i = 0; i < pInput.Length; i++)
        {
            if (i >= numColliders)
            {
                if (pInput[i] != null)
                {
                    //stop showing UI to the player that left
                    interactUI.ShowIconToPlayer(false, pInput[i].playerNum);
                }

                pInput[i] = null;
            }
        }
    }
}

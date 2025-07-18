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
    [SerializeField] private bool requireItem = false;
    [SerializeField] private string validItemTag;
    private bool canInteract = true;

    private int interactCount = 0;

    private bool setupComplete = false;

    [SerializeField] private PlayerData[] playerData;
    [SerializeField] private Collider[] players;
    

    private bool colliding = false;

    [SerializeField] private LayerMask playerMask;

    private void Start()
    {
        //initialize the array that holds players in range
        players = new Collider[2];


        //create the playerData array
        playerData = new PlayerData[2];

        playerData[0] = new PlayerData();
        playerData[1] = new PlayerData();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }

    private void Update()
    {

        if (colliding && canInteract)
        {
            for (int i = 0; i < playerData.Length; i++)
            {
                if (playerData[i] != null && playerData[i].pInput != null)
                {
                    //a player has pressed the crouch input
                    if (playerData[i].pInput.interactPressed && playerData[i].canInteract)
                    {
                        //interact with this object and pass in the player object
                        OnInteract.Invoke();

                        if (requireItem)
                        {
                            ItemBase item = playerData[i].pBag.activeItemBase;
                            playerData[i].pBag.RemoveItem(item);
                            Destroy(item.gameObject, 0.1f);
                        }

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

    public virtual void DetectPlayers()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, interactRange, players, playerMask);

        if (numColliders > 0)
        {
            colliding = true;
        }
        else
        {
            colliding = false;
        }

        //players outputs the active objects in the array
        //remove old colliders from the array
        for (int i = 0; i < players.Length; i++)
        {
            int pNum;

            if (players[i] != null)
            {
                pNum = players[i].GetComponentInParent<PlayerInputDetection>().playerNum;
                //if the player has not been setup yet, set it up
                if (playerData[pNum - 1].pInput == null)
                {
                    playerData[pNum - 1].pInput = players[i].GetComponentInParent<PlayerInputDetection>();
                    playerData[pNum - 1].pBag = players[i].GetComponentInParent<Bag>();
                }
            }

            //if the current loop of the player is greater than the number of colliders, clear out the newest addition
            if (i >= numColliders && players[i] != null)
            {
                pNum = players[i].GetComponentInParent<PlayerInputDetection>().playerNum;
                playerData[pNum - 1].canInteract = false;
                interactUI.ShowIconToPlayer(false, pNum);

                //here is where a player gets deleted
                players[i] = null;

                //clear out both players and reset them both
                //this is neccesary because theres some weird issues where the wrong player will leave because of the order of how the array is handled
                //I don't know a solution other than reworking the way this script works so for now this works
                playerData[0].canInteract = false;
                interactUI.ShowIconToPlayer(false, 1);
                playerData[1].canInteract = false;
                interactUI.ShowIconToPlayer(false, 2);
            }
            else if (players[i] != null)//if the current index is currently in the number of colliders 
            {
                pNum = players[i].GetComponentInParent<PlayerInputDetection>().playerNum;

                if (requireItem)
                {
                    if (playerData[pNum - 1].pBag.activeItemBase != null)
                    {
                        if (playerData[pNum - 1].pBag.activeItemBase.CompareTag(validItemTag))
                        {
                            interactUI.ShowIconToPlayer(true, pNum);
                            playerData[pNum - 1].canInteract = true;
                        }
                        else
                        {
                            playerData[pNum - 1].canInteract = false;
                            interactUI.ShowIconToPlayer(false, pNum);
                        }
                    }
                    else
                    {
                        playerData[pNum - 1].canInteract = false;
                        interactUI.ShowIconToPlayer(false, pNum);
                    }
                   
                }
                else
                {
                    //this player can interact, show them the icon
                    interactUI.ShowIconToPlayer(true, pNum);
                    playerData[pNum - 1].canInteract = true;
                }
            }
        }


    }

}

[System.Serializable]
public class PlayerData
{
    public PlayerInputDetection pInput = null;
    public Bag pBag = null;
    public bool canInteract = true;
}

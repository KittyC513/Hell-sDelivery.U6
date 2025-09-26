using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
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


    private void Update()
    {
        if (limitInteracts)
        {
            if (interactCount >= maxNumberOfInteracts && canInteract == true)
            {
                canInteract = false;
            }
        }
    }

    public void EnableUI(int playerNum)
    {
        interactUI.ShowIconToPlayer(true, playerNum);
    }

    public void DisableUI(int playerNum)
    {
        interactUI.ShowIconToPlayer(false, playerNum);
    }

    public void Interact(PlayerInputDetection playerInput)
    {
        if (canInteract)
        {
            //interact with this object 
            OnInteract.Invoke();

            if (requireItem)
            {
                //Destroy(item.gameObject, 0.1f);
            }

            interactCount++;
        }
    }

}

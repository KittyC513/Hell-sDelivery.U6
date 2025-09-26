using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private PlayerInputDetection inputDetection;
    [SerializeField] private float interactRange = 3;
    [SerializeField] private LayerMask interactMask;

    private InteractableObject lastInteractable;

    private void Update()
    {
        //look nearby for interactables
        //interact with one interactable (the closest one)
        if (GetInteractable() != null)
        {
            if (inputDetection.interactPressed)
            {
                GetInteractable().Interact(inputDetection);
            }
        }
    }

    //Gets nearby interactable objects and returns the closest
    private InteractableObject GetInteractable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange, interactMask);

        if (colliders.Length > 0)
        {
            float closestDist = interactRange + 1;
            GameObject closestObj = colliders[0].gameObject;

            //check all the colliders to find the closest one
            for (int i = 0; i < colliders.Length; i++)
            {
                float dist = Vector3.Distance(transform.position, colliders[i].transform.position);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestObj = colliders[i].gameObject;
                }
            }

            if (lastInteractable != null)
            {
                if (closestObj != lastInteractable)
                {
                    //disable the ui on the last interactable
                    lastInteractable.DisableUI(inputDetection.playerNum);

                    //set the new interactable
                    lastInteractable = closestObj.GetComponent<InteractableObject>();
                    //enable the UI for the new interactable
                    lastInteractable.EnableUI(inputDetection.playerNum);
                }
            }
            else
            {
                //set the new interactable
                lastInteractable = closestObj.GetComponent<InteractableObject>();
                //enable the UI for the new interactable
                lastInteractable.EnableUI(inputDetection.playerNum);
            }
            
            return closestObj.GetComponent<InteractableObject>();
        }
        else
        {
            if (lastInteractable != null)
            {
                //disable the ui on the last interactable
                lastInteractable.DisableUI(inputDetection.playerNum);

                lastInteractable = null;
            }
          
            return null;
        }
      
    }
}

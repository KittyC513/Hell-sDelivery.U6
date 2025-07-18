using UnityEngine;

public class RequiredObjInteractable : InteractableObject
{
    ////check if the player has an item in hand
    ////reference to the player's bags
    //[SerializeField] private Bag[] pBag;
    //[SerializeField] private string[] acceptedObjectTags;

    //private void Awake()
    //{
    //    pBag = new Bag[2];
    //}

    //public override void DetectPlayers()
    //{
    //    int numColliders = Physics.OverlapSphereNonAlloc(transform.position, interactRange, players, playerMask);

    //    //check if any collisions are happening
    //    if (numColliders > 0)
    //    {
    //        colliding = true;
    //    }
    //    else
    //    {
    //        colliding = false;
    //    }

    //    //numColliders is how many players are colliding
    //    //0 is always going to be the first player to collide
    //    for (int i = 0; i < numColliders; i++)
    //    {
    //        //set the player input
    //        pInput[i] = players[i].GetComponentInParent<PlayerInputDetection>();

    //        //set the player's bag
    //        pBag[i] = players[i].GetComponentInParent<Bag>();

    //        if (CheckObjectTag(pBag[i].activeItemBase.gameObject.tag))
    //        {
    //            //the player has the correct object

    //            //check which player is colliding and show ui to them
    //            interactUI.ShowIconToPlayer(true, pInput[i].playerNum);
    //        }
    //    }

    //    for (int i = 0; i < pInput.Length; i++)
    //    {
    //        if (i >= numColliders)
    //        {
    //            if (pInput[i] != null)
    //            {
    //                //stop showing UI to the player that left
    //                interactUI.ShowIconToPlayer(false, pInput[i].playerNum);
    //            }

    //            pBag[i] = null;
    //            pInput[i] = null;
    //        }
    //    }
        
    //}

    //private bool CheckObjectTag(string tag)
    //{
    //    for (int i = 0; i < acceptedObjectTags.Length; i++)
    //    {
    //        if (tag == acceptedObjectTags[i])
    //        {
    //            return true;
    //        }
    //    }
        
    //    return false;
    //}
}



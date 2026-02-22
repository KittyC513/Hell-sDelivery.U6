using System.Collections.Generic;
using UnityEngine;

public class ProximityUI : MonoBehaviour
{
    //this script will work seperate from interactables and will just be an easy way to toggle billboard ui on or off based on a trigger enter + exit function
    [SerializeField] private BillboardUI billboardUI;
    private List<GameObject> players;

    private void Start()
    {
        players = new List<GameObject>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCollider"))
        {
            PlayerInputDetection inputDetection;
            inputDetection = other.GetComponentInParent<PlayerInputDetection>();

            players.Add(other.gameObject);

            EnableUI(inputDetection.playerNum);
        }
       
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerCollider"))
        {
            PlayerInputDetection inputDetection;
            inputDetection = other.GetComponentInParent<PlayerInputDetection>();

            DisableUI(inputDetection.playerNum);

            Debug.Log("Exited");
        }
    }

    public void EnableUI(int playerNum)
    {
        billboardUI.ShowIconToPlayer(true, playerNum);
    }

    public void DisableUI(int playerNum)
    {
        billboardUI.ShowIconToPlayer(false, playerNum);
    }

}

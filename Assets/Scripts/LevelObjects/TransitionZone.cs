using UnityEditor;
using UnityEngine;

public class TransitionZone : MonoBehaviour
{
    private int activePlayers = 0;
    [SerializeField] private int requiredPlayers = 2;
    [SerializeField] private TransitionScenes transitionScenes;
    [SerializeField] private bool canTransition = true;


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activePlayers += 1;

        }
        
        if (activePlayers >= requiredPlayers && canTransition)
        {
            canTransition = false;
            transitionScenes.StartTransition();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activePlayers -= 1;

        }
    }

  
}

using System;
using UnityEngine;
using UnityEngine.Events;

public class SummoningCircle : MonoBehaviour
{
    private bool isActive = false;
    private PlayerInputDetection currentPlayer;
    private Vector3 playerStartPos;
    private float timeElapsed;
    [SerializeField] private float playerRepositionSpeed = 1;

    //called once on summon enter
    [SerializeField] public UnityEvent OnSummonActivate;
    //called every frame while the summon is active
    [SerializeField] public UnityEvent WhileSummonActive;
    //called once when the summon is ended
    [SerializeField] public UnityEvent OnSummonExit;
    private InteractableObject interactableObject;
    [SerializeField] private SoundEffectPlayer sfxPlayer;
    [SerializeField] private Animator anim;
    
    
    

    public void StartSummon(PlayerInputDetection player, InteractableObject interactable)
    {
        currentPlayer = player;
        isActive = true;

        anim.SetBool("Activate", true);

        //grab the player position to lerp
        playerStartPos = currentPlayer.transform.position;
        //invoke unity event
        OnSummonActivate.Invoke();
        //reset timer for the lerp
        timeElapsed = 0;

        interactableObject = interactable;
        interactable.canInteract = false;

        sfxPlayer.PlaySoundEffect("ObjectBank1", "SummoningActivate");
        sfxPlayer.QueueSoundEffect("ObjectBank1", "SummoningActive", 0.2f);

        //freeze the player interacting with this object
        if (currentPlayer.playerNum == 1) 
        {
            //GameManager.instance.FreezePlayer1();
        }
        else 
        {
            //GameManager.instance.FreezePlayer2();
        }

        currentPlayer.GetComponent<PlayerStateMachine>().OverrideState(PlayerStateMachine.PlayerStates.summon);
    }

    private void Update()
    {
        if (isActive)
        {
            WhileActive();
        }
    }

    private void WhileActive()
    {
        timeElapsed += Time.deltaTime * playerRepositionSpeed;

        WhileSummonActive.Invoke();

        //move the player towards the center of the circle
        float percent = Mathf.Clamp(timeElapsed, 0, 10) / 1;
        currentPlayer.transform.position = Vector3.Lerp(playerStartPos, transform.position + new Vector3(0, 1.1f, 0), percent);

        //current player lets go of the interact button
        if (!currentPlayer.interactHeld)
        {
            //end the summon
            ExitSummon();
        }
    }

    private void ExitSummon()
    {
        sfxPlayer.StopAllSoundEffects();
        Debug.Log("Exit Summon");

        anim.SetBool("Activate", false);

        sfxPlayer.PlaySoundEffect("ObjectBank1", "SummoningDeactivate");

        //sfxPlayer.StopAudioDelayed(2);

        //unfreeze the player interacting with this object
        if (currentPlayer.playerNum == 1) 
        {
            //GameManager.instance.UnFreezePlayer1();
        }
        else 
        {
            //GameManager.instance.UnFreezePlayer2();
        }

        currentPlayer.GetComponent<PlayerStateMachine>().UnFreezeStateMachine();
        interactableObject.canInteract = true;
        currentPlayer = null;
        isActive = false;
        OnSummonExit.Invoke();
    }
}

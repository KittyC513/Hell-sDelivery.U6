using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//we inherit from the stateManager class which holds the functionality in order to manage our current state
public class PlayerStateMachine : StateManager<PlayerStateMachine.PlayerStates>
{
    //create states and add them to the dictionary 
    public enum PlayerStates { grounded, jump, doubleJump, airborne, sliding, ledgeHang, frozen, ragdoll, attack, }

    public PlayerController controller;
    public PlayerAttackControl attackControl;

    private PlayerStates defaultState;
    [SerializeField] private PlayerStates showCurrentState;

    private void Awake()
    {
        //this is how you initialize and add a state to the dictionary
        //you assign your enum first followed by the script you will assign to that enum
        states.Add(PlayerStates.airborne, new PlayerAirborneState(PlayerStates.airborne, controller));
        states.Add(PlayerStates.grounded, new PlayerGroundedState(PlayerStates.grounded, controller));
        states.Add(PlayerStates.jump, new PlayerJumpState(PlayerStates.jump, controller));
        states.Add(PlayerStates.doubleJump, new DoubleJumpState(PlayerStates.doubleJump, controller));
        states.Add(PlayerStates.sliding, new PlayerSlidingState(PlayerStates.sliding, controller));
        states.Add(PlayerStates.ledgeHang, new PlayerLedgeHangState(PlayerStates.ledgeHang, controller));
        states.Add(PlayerStates.frozen, new PlayerFrozenState(PlayerStates.frozen, controller));
        states.Add(PlayerStates.ragdoll, new PlayerRagdollState(PlayerStates.ragdoll, controller));
        states.Add(PlayerStates.attack, new PlayerAttackState(PlayerStates.attack, controller, attackControl));

        //set our current state to airborne as a default starting state
        currentState = states[PlayerStates.airborne];
        defaultState = PlayerStates.airborne;
    }

    private void LateUpdate()
    {
        showCurrentState = currentState.stateKey;
    }

    //this function can change the current state without asking or interfering with the current state
    public void OverrideState(PlayerStates targetState)
    {
        if (currentState.stateKey != targetState) TransitionToState(targetState);
    }

    //this will set the state machine to a frozen state
    public void FreezeStateMachine()
    {
        TransitionToState(PlayerStates.frozen);
    }

    //this will reset the state machine to an unfrozen state
    public void UnFreezeStateMachine()
    {
        TransitionToState(defaultState);
    }

}

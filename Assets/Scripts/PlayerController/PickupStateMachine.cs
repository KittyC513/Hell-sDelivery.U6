using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupStateMachine : StateManager<PickupStateMachine.PickupStates>
{
    //create states and add them to the dictionary 
    public enum PickupStates { empty, pickup, holding, throwing }

    public PlayerObjectController objectController;
    public PlayerController playerController;

    private void Awake()
    {
        //this is how you initialize and add a state to the dictionary
        //you assign your enum first followed by the script you will assign to that enum
        states.Add(PickupStates.empty, new PickupEmptyState(PickupStates.empty, objectController));
        states.Add(PickupStates.pickup, new PickupPickingState(PickupStates.pickup, objectController));
        states.Add(PickupStates.holding, new PickupHoldingState(PickupStates.holding, objectController));
        states.Add(PickupStates.throwing, new PickupThrowingState(PickupStates.throwing, objectController));

        //set our current state to airborne as a default starting state
        currentState = states[PickupStates.empty];
    }
}

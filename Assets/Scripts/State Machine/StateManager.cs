using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class is used to create new statemanagers, inherit from this class to create a new state manager
//the state manager uses monobehaviour functionality such as start, update etc. to run the functions in our states
public abstract class StateManager<Estate> : MonoBehaviour where Estate : Enum
{
    //Create a dictionary that requires an enum (Estate) and outputs a BaseState class 
    protected Dictionary<Estate, BaseState<Estate>> states = new Dictionary<Estate, BaseState<Estate>>();

    protected BaseState<Estate> currentState;
    

    protected bool isTransitioningState = false;
    void Start() 
    {
        currentState.EnterState();
        
    }

    void Update() 
    {
        Estate nextStateKey = currentState.GetNextState();

        //if our next state key is the same as our current state, keep updating our current state
        if (!isTransitioningState && nextStateKey.Equals(currentState.stateKey))
        {
            currentState.UpdateState();
        }
        else if (!isTransitioningState) //otherwise if our next state key doesnt match, change our current state to the state associated with the new state key
        {
            TransitionToState(nextStateKey);
        }
        
    }

    private void FixedUpdate()
    {
        if (!isTransitioningState)
        {
            currentState.PhysicsUpdate();
        }
    }

    public void TransitionToState(Estate stateKey)
    {
        isTransitioningState = true;

        
        //exit the current state
        currentState.ExitState();

        //update to the new state by using our dictionary
        currentState = states[stateKey];

        //play the start function on our new state
        currentState.EnterState();

        isTransitioningState = false;
    }

    void OnTriggerEnter (Collider other)
    {
        currentState.OnTriggerEnter(other);
    }

    void OnTriggerStay(Collider other) 
    {
        currentState.OnTriggerStay(other);
    }

    void OnTriggerExit(Collider other) 
    {
        currentState.OnTriggerExit(other);
    }
  

}

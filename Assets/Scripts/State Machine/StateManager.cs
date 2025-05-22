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

    [SerializeField] protected Animator anim;

    //need the base state to be able to call a function that can change the current anim
    //or be able to trigger it in some way through this script
    //this script has access to the current base state
    //if something changes in the current base state this can listen
    //if the base state has a variable for a current animation
    //solution could be to check if that animation string changes from " " and if it does then change the current animation to the new string
    //afterwards resetting the string to " " so that if it needs to be set again it can be set again but it won't constantly keep setting the new animation

    //now this works but how do i get access to an animator without null reference and what if a state machine doesn't need access or have access to an animator

    //could make a serialized field that you can fill and if left empty the animation functionality will be ignored

    //or i could make a variation of state manager that requires an animator??
    

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

            //check for changes in animation state
            if (currentState.animName != " " && anim != null)
            {
                ChangeAnimation(currentState.animName);
            }
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

        //update the animator
        if (currentState.animName != " " && anim != null)
        {
            ChangeAnimation(currentState.animName);
        }

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
  
    private void ChangeAnimation(string animationName)
    {
        anim.Play(animationName);
        currentState.animName = " ";
    }
}

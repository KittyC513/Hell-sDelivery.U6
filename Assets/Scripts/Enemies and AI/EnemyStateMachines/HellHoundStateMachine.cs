using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using static PlayerStateMachine;

public class HellHoundStateMachine : StateManager <HellHoundStateMachine.HoundStates>
{
    public enum HoundStates { wander, chase, attack, cooldown, die, takeHit, lunge }

    [SerializeField] private HellHoundBase houndBase;

    [SerializeField] public HoundStates showCurrentState;

    private void Awake()
    {
        //initialize states for the state machine
        states.Add(HoundStates.wander, new HellHoundWander(HoundStates.wander, houndBase));
        states.Add(HoundStates.chase, new HellHoundChase(HoundStates.chase, houndBase));
        states.Add(HoundStates.attack, new HellHoundAttack(HoundStates.attack, houndBase));
        states.Add(HoundStates.cooldown, new HellHoundCooldown(HoundStates.cooldown, houndBase));
        states.Add(HoundStates.takeHit, new HellHoundTakeHit(HoundStates.takeHit, houndBase));
        states.Add(HoundStates.lunge, new HellHoundLunge(HoundStates.lunge, houndBase));
        //set current state
        currentState = states[HoundStates.wander];
        TransitionToState(HoundStates.wander);
    }



    public void OverrideState(HoundStates newState)
    {
        TransitionToState(newState);
    }

    public void OnHoundDeath()
    {
        OverrideState(HoundStates.die);
    }

    private void LateUpdate()
    {
        showCurrentState = currentState.stateKey;
    }

}


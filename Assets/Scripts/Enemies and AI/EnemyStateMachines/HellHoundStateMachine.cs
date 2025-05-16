using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;

public class HellHoundStateMachine : StateManager <HellHoundStateMachine.HoundStates>
{
    public enum HoundStates { wander, chase, attack, hurt, die }

    public EnemyHealth enemyHealth;
    [SerializeField] private NavMeshAgent navAgent;

    [SerializeField] private float maxWanderDistance;

    private void Start()
    {
        //this is the syntax for using a generic state such as NavmeshWander
        //because we may want other things to wander around on a nav mesh instead of declaring the exact type of enum such as PlayerStates.X 
        //its a generic state called State, now in the state manager script (this one) we can manually set the generic to HoundStates
        //the < > after the new NavmeshWander is where you specify which Enum will replace the generic one
        states.Add(HoundStates.wander, new NavmeshWander<HoundStates>(HoundStates.wander, navAgent, maxWanderDistance));

        currentState = states[HoundStates.wander];
    }
}


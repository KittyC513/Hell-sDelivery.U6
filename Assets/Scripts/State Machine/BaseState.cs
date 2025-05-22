using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is the blueprint for a state which can be inhereted from to create a new state
public abstract class BaseState<Estate> where Estate : Enum
{
    public BaseState(Estate key)
    {
        stateKey = key;
    }

    public Estate stateKey { get; private set; }
    public string animName = " ";

    public abstract void EnterState();
    public virtual void PhysicsUpdate() { }
    public abstract void ExitState();
    public abstract void UpdateState();

    //Estate is a generic, its a class with a placeholder data type that can be changed when it is assigned
    //we restrict the type by adding where Estate: Enum meaning that Estate must be an enum 
    public abstract Estate GetNextState();

    public virtual void OnTriggerEnter(Collider other) { }
    public virtual void OnTriggerExit(Collider other) { }
    public virtual void OnTriggerStay(Collider other) { }

}

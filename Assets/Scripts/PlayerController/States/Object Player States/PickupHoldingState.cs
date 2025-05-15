using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHoldingState : BaseState<PickupStateMachine.PickupStates>
{

    private PlayerObjectController oControl;
    public PickupHoldingState(PickupStateMachine.PickupStates key, PlayerObjectController controller) : base(key)
    {
        oControl = controller;
    }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {

    }

    public override PickupStateMachine.PickupStates GetNextState()
    {
        if (oControl.GetThrowInput())
        {
            return PickupStateMachine.PickupStates.throwing;
        }
        return stateKey;
    }

    public override void UpdateState()
    {
        oControl.currentObject.transform.position = oControl.HoldPoint.position;
    }
}

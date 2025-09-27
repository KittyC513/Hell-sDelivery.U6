
using UnityEngine;

public class PlayerSpinFall : PlayerAirborneState
{
    private Quaternion startRotation;

    public PlayerSpinFall(PlayerStateMachine.PlayerStates key, PlayerController controller) : base(key, controller)
    {
        pControl = controller;
        ledgeGrabHorizontalRange = pControl.LedgeGrabHorizontalRange;
        ledgeGrabUpwardsRange = pControl.LedgeGrabUpwardsRange;
        ledgeGrabMask = pControl.LedgeGrabMask;
    }

    public override void EnterState()
    {
        ledgeDetected = false;
        rb = pControl.RB;
        playerHitboxHeight = pControl.PlayerHitboxHeight;
        goalVelocityChange = Vector3.zero;
        maxFallSpeed = pControl.MaxSpinFallSpeed;
        fallAccel = pControl.SpinFallAccel;
        startRotation = pControl.PlayerModel.transform.localRotation;
        //set animation 
        animName = "Idle";

    }

    public override void ExitState()
    {
        ledgeDetected = false;

        //if we touch the ground make us grounded
        if (pControl.PlayerModel != null)
        {
            //rotate our player 360 degrees over the attack duration
            pControl.PlayerModel.transform.localRotation = startRotation;
        }
    }

    public override void UpdateState()
    {
        //pControl.PlayerModel.transform.rotation = startRotation * Quaternion.AngleAxis((Time.deltaTime / 0.15f) * 360f, Vector3.up);
        pControl.PlayerModel.transform.Rotate(Vector3.up, (Mathf.Lerp(1500, 600, rb.linearVelocity.y / -maxFallSpeed)) * Time.deltaTime);

    }
}

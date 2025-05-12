using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    // Reference to the PlayerJoinHandler script where OnJoinPressed is defined
    public PlayerJoinHandler playerJoinHandler;

    // This method will be invoked when the "Join" action is triggered
    public void OnJoinInput(InputAction.CallbackContext context)
    {
        if (context.started) // If the action is pressed (can use started, performed, or canceled)
        {
            playerJoinHandler.OnJoinPressed(); // Trigger the join logic from the PlayerJoinHandler
        }
    }
}

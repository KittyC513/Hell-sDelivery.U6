using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputDetection : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputActionAsset inputActionAsset;

    private InputAction leftStick;
    private InputAction jump;
    private InputAction Crouch;

    private bool jumpValue;

    private float jumpHoldValue;

    private InputActionMap playerMap;

    private void Awake()
    {
        inputActionAsset = GetComponent<PlayerInput>().actions;
        playerInput = GetComponent<PlayerInput>();
        playerMap = inputActionAsset.FindActionMap("Player");
    }

    private void OnEnable()
    {
        playerMap.FindAction("Jump").started += Jump;
        playerMap.FindAction("Jump").canceled += JumpCanceled;
    }

    private void OnDisable()
    {
        playerMap.FindAction("Jump").started -= Jump;
        playerMap.FindAction("Jump").canceled -= JumpCanceled;
    }

    private void Update()
    {
        
    }

    //this function runs when you press jump
    private void Jump(InputAction.CallbackContext action)
    {
        Debug.Log("Jump Pressed");
    }

    private void JumpCanceled(InputAction.CallbackContext action)
    {
        Debug.Log("Jump Let go");
    }
}

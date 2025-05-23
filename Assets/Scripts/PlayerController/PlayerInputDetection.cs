using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public enum E_InputDeviceType 
{
    keyboard,
    Gamepad,
}

public class PlayerInputDetection : NetworkBehaviour
{
    private PlayerInput playerInput;
    private InputActionAsset inputActionAsset;

    private InputActionMap playerMap;
    [HideInInspector] public bool jumpPressed;
    [HideInInspector] public bool crouchPressed;
    [HideInInspector] public bool lockPressed;
    [HideInInspector] public bool attackPressed;
    [HideInInspector] public bool grabPressed;
    private Vector2 horizontalInputValue;
    private InputAction moveAction;
    private InputAction lookAction;

    public Camera cam;
    public Camera playerCam;

    [SerializeField] private float jumpBufferTime = 0.2f; //how long the jump input is read, used to buffer jumps 
    private float jumpBufferCurrent = 0;

    [SerializeField] private float dropActiveItemTime = 0.3f;
    public float currentTime = 0;

    [Header("Device Check")]
    public bool isCheckedDevice;
    public E_InputDeviceType inputDeviceType;

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

        playerMap.FindAction("Crouch").started += Crouch;
        playerMap.FindAction("Crouch").canceled += CrouchCanceled;

        playerMap.FindAction("Lock").started += Lock;
        playerMap.FindAction("Lock").canceled += LockCanceled;

        playerMap.FindAction("Attack").started += AttackPressed;
        playerMap.FindAction("Attack").canceled += AttackCanceled;

        playerMap.FindAction("Grab").started += GrabPressed;
        playerMap.FindAction("Grab").canceled += GrabCanceled;


        moveAction = playerMap.FindAction("Move");
        lookAction = playerMap.FindAction("Look");
    }

    private void OnDisable()
    {
        playerMap.FindAction("Jump").started -= Jump;
        playerMap.FindAction("Jump").canceled -= JumpCanceled;

        playerMap.FindAction("Crouch").started -= Crouch;
        playerMap.FindAction("Crouch").canceled -= Crouch;

        playerMap.FindAction("Lock").started -= Lock;
        playerMap.FindAction("Lock").canceled -= LockCanceled;

        playerMap.FindAction("Attack").started -= AttackPressed;
        playerMap.FindAction("Attack").canceled -= AttackCanceled;

        playerMap.FindAction("Grab").started -= GrabPressed;
        playerMap.FindAction("Grab").canceled -= GrabCanceled;
    }

    private void Start()
    {
        InputDeviceCheck();

        if(NGO_PanelControl.instance != null)
            NGO_PanelControl.instance.inputDetector = this;
    }
    public Vector3 GetHorizontalMovement()
    {
        return GetRelativeInputDirection(cam, horizontalInputValue = moveAction.ReadValue<Vector2>());
    }

    public Vector2 GetCameraMovement()
    {
        return lookAction.ReadValue<Vector2>();
    }

    //this function runs when you press jump
    private void Jump(InputAction.CallbackContext action)
    {
        //reset buffer time on jump press
        jumpBufferCurrent = Time.time;
        jumpPressed = true;
    }

    private void JumpCanceled(InputAction.CallbackContext action)
    {
        //if jump is let go set bool to false
        jumpPressed = false;
    }

    public bool JumpBuffered()
    {
        if (Time.time - jumpBufferCurrent < jumpBufferTime)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Crouch(InputAction.CallbackContext action)
    {
        crouchPressed = true;
    }

    private void CrouchCanceled(InputAction.CallbackContext action)
    {
        crouchPressed = false;
    }

    private void Lock(InputAction.CallbackContext action)
    {
        lockPressed = true;
    }

    private void LockCanceled(InputAction.CallbackContext action)
    {
        lockPressed = false;
    }

    private void AttackPressed(InputAction.CallbackContext action)
    {
        attackPressed = true;
    }

    private void AttackCanceled(InputAction.CallbackContext action)
    {
        attackPressed = false;
    }

    private void GrabPressed(InputAction.CallbackContext action)
    {
        grabPressed = true;


    }

    public bool IsDropActiveItem()
    {
        if (grabPressed)
        {
            //Measure pressing buttom time
            if (currentTime >= dropActiveItemTime)
            {
                dropActiveItemTime = currentTime;
                return true;
            }
            else
            {
                currentTime += Time.deltaTime;
                return false;
            }
        }
        
        return false;
    }

    private void GrabCanceled(InputAction.CallbackContext action)
    {
        grabPressed = false;
        currentTime = 0;
    }

    private Vector3 GetRelativeInputDirection(Camera camera, Vector2 inputValue)
    {
        if (camera != null)
        {
            //get camera forward and right
            Vector3 camForward = camera.transform.forward;
            Vector3 camRight = camera.transform.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward = camForward.normalized;
            camRight = camRight.normalized;

            //get our stick input
            Vector3 stickInput = inputValue;
            stickInput = stickInput.normalized;
            //multiply our stick value by our cam right and forward to get a camera relative input
            Vector3 horizontal = stickInput.x * camRight;
            Vector3 vertical = stickInput.y * camForward;

            Vector3 input = horizontal + vertical;
            input = input.normalized;

            return input.normalized;
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }
    #region Player input device check
    private void InputDeviceCheck()
    {
        
        if (!isCheckedDevice)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                inputDeviceType = E_InputDeviceType.keyboard;         
                //Cursor.visible = false;

                isCheckedDevice = true;
            }
            else if (Gamepad.current != null && Gamepad.current.aButton.wasPressedThisFrame)
            {
                inputDeviceType = E_InputDeviceType.Gamepad;
                //Cursor.lockState = CursorLockMode.Locked;
                isCheckedDevice = true;
            }
        }
    }
    #endregion
}

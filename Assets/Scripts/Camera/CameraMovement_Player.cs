//Comment out `undef` or `def Newwork` to switch between script versions
// need to be set on PlayerController as well
#define Network
#undef Network

using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class CameraMovement_Player : NetworkBehaviour
{
    public Transform playerTransform;
    public PlayerInputDetection inputDetection;
    public PlayerLockOn playerLockOn;
    public Vector3 camOffset;
    public Vector3 resetCamOffset;

    [Header("Camera Variables")]
    Vector3 mDefaultDir;
    [Tooltip("rotate value")]
    Vector3 mRotateValue;
    //横向:偏航角 up_down rotate
    Vector3 mPitchRotateAxis;
    //纵向:俯仰角 left_right rotate
    Vector3 mYawRotateAxis;
    public float distance = 4;
    public float oriDistance = 4;
    public float topdownDistance;
    public float rotateSpeed = 120f;
    public Vector3 offset = new Vector3(0f, 1.5f, 0f);

    public bool invertPitch;
    public Vector2 pitchLimit = new Vector2(-40f, 70f);
    public Vector2 pitchLimitCD = new Vector2(-10f, 50f);

    private Vector2 inputDelta;
    private Quaternion horizontalQuat;
    private Quaternion verticalQuat;
    private Vector3 finalDir;

    private Vector3 from;
    private Vector3 to;

    private Vector3 exceptTo;
    private float expectDistance;

    private Vector3 dir;
    private RaycastHit hit;
    private bool isHit;

    [Header("Keyboard variables")]
    public float keyboardMoveSpeed = 0.3f;

    [Header("Collision Detection Method")]
    private int[] layerID;
    public LayerMask obstacleMask;
    public float detectorSphereRadius = 0.3f;

    [Header("Auto adjustment interpolation")]
    float mCurrentDistance;
    float mDistanceRecoveryDelayCounter;

    public float distanceRecoverySpeed = 3f;
    public float distanceRecoveryDelay = 1f;

    [Header("Camera Moving Speed")]
    public Vector3 movePos;
    public float moveSpeed = 5f;
    public float moveSpeed_coneSight = 7f;
    public float oriMoveSpeed;
    public float topdownMoveSpeed;
    public Transform defaultPos;
    public bool resetPos = false;
    public bool resetCamPos = false;
    public float defaultPitch = -15f;

    public Vector3 resetCamMovePos;

    public CameraManager cameraManager;
    public float offsetY;

    private PlayerSettings playerSettings;

    public bool hasInitialisedConeCam = false;
    public bool didSyncAfterReset = false;
    public bool isBelndingToCone = false;
    public bool wasConeMode = false;
    Coroutine blendRoutine;

    public float normalToConeBlendTime = 0.25f;

    private void Start()
    {
        distance = oriDistance;
        moveSpeed = oriMoveSpeed;
        // get y axis
        var upAxis = -Physics.gravity.normalized;
        //set cam default regarding player's position
        mDefaultDir = Vector3.ProjectOnPlane(transform.position - playerTransform.position, upAxis).normalized;
        //Initial yam and pitch axis
        mYawRotateAxis = upAxis;
        mPitchRotateAxis = Vector3.Cross(upAxis, Vector3.ProjectOnPlane(transform.forward, upAxis));
        
        //reference the player settings script
        playerSettings = PlayerSettings.instance;

        if(playerSettings != null)
        {
            playerSettings.onSettingsChange += UpdateSensitivity;
            UpdateSensitivity();
        }


#if Network

        if (!IsOwner)
            this.gameObject.SetActive(false);
#endif
    }
    private void OnEnable()
    {
        /*************/
        //distance = topdownDistance;
        /*************/


        //Reset function 
        mRotateValue.x = playerTransform.eulerAngles.y; // face behind player
        mRotateValue.y = Mathf.Clamp(distance, pitchLimit.x, pitchLimit.y); // default pitch

        this.transform.position = cameraManager.lockCam.transform.position;
    }

    private void OnDisable()
    {
        if (playerSettings != null)
        {
            playerSettings.onSettingsChange -= UpdateSensitivity;
        }
    }

    private void Update()
    {
        
    }

    //update the camera rotate speed based on the PlayerSettings sensitivty values
    //is called on settingsChange via events
    private void UpdateSensitivity()
    {
        if (playerSettings != null)
        {
            if (inputDetection.playerNum == 1)
            {
                if (playerSettings.p1Sensitivity != rotateSpeed)
                {
                    rotateSpeed = playerSettings.p1Sensitivity;
                }

                invertPitch = playerSettings.p1InvertCam;
            }
            else
            {
                if (playerSettings.p2Sensitivity != rotateSpeed)
                {
                    rotateSpeed = playerSettings.p2Sensitivity;
                }

                invertPitch = playerSettings.p2InvertCam;
            }
        }
    }


    void LateUpdate()
    {
        bool condMode = playerLockOn.isWithDetonator && inputDetection.lockPressed;
        if (inputDetection.isExploded && !inputDetection.isResetCam)
        {
            SwitchToTopDownCam();
            inputDetection.isResetCam = true;
        }
        else if (!inputDetection.isExploded && (distance != oriDistance || moveSpeed != oriMoveSpeed))
        {
            distance = oriDistance;
            moveSpeed = oriMoveSpeed;
        }

        if (condMode)
        {
            if (!wasConeMode)
            {
                wasConeMode = true;

                resetCamPos = false;
                hasInitialisedConeCam = false;
                BeginConeTransition(true);
            }

            if(!isBelndingToCone)
                ConeSightCamMovement();
        }
        else
        {
            wasConeMode = false;
            CameraMovement();
            hasInitialisedConeCam = false;
        }

    }
    #region Topdown Cam while gaining exploded force
    void SwitchToTopDownCam()
    {
        Debug.Log("Topdown view");
        distance = topdownDistance;
        moveSpeed = topdownMoveSpeed;

        mRotateValue.x = playerTransform.eulerAngles.y; // face behind player
        mRotateValue.y = Mathf.Clamp(distance, pitchLimit.x + offsetY, pitchLimit.y); // default pitch

    }
    #endregion


    #region Player Camera movement(Base)
    void CameraMovement()
    {
        if (playerTransform == null) return;

        if (!resetCamPos)
        {
            mRotateValue.x = playerTransform.eulerAngles.y; // face behind player
            mRotateValue.y = Mathf.Clamp(defaultPitch, pitchLimit.x, pitchLimit.y); // default pitch

            Quaternion initRot = Quaternion.Euler(mRotateValue.y, mRotateValue.x, 0f);
            resetCamMovePos = playerTransform.position + initRot * resetCamOffset;
            //transform.position = initPos;
            //transform.rotation = initRot;
            transform.position = Vector3.Lerp(this.transform.position, resetCamMovePos, Time.deltaTime * moveSpeed);

            // smooth rotation during reset
            Vector3 lookFrom = playerTransform.localToWorldMatrix.MultiplyPoint3x4(offset);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookFrom - transform.position), Time.deltaTime * rotateSpeed);

            // If player moves camera input, exit reset mode
            inputDelta = new Vector2(
                inputDetection.inputDeviceType == E_InputDeviceType.gamepad ? inputDetection.GetCameraMovement().x : inputDetection.GetCameraMovement().x * keyboardMoveSpeed,
                inputDetection.inputDeviceType == E_InputDeviceType.gamepad ? inputDetection.GetCameraMovement().y : inputDetection.GetCameraMovement().y * keyboardMoveSpeed
            );

            if (inputDelta.sqrMagnitude > 0.0001f || inputDetection.GetHorizontalMovement().sqrMagnitude > 0.0001f)
            {
                resetCamPos = true;
                didSyncAfterReset = false;
            }

            return; 
        }

        if (!didSyncAfterReset)
        {
            SyncRotateValueFromCamera();
            didSyncAfterReset = true;
        }


        //get input value
        inputDelta = new Vector2(inputDetection.inputDeviceType == E_InputDeviceType.gamepad ?
                        inputDetection.GetCameraMovement().x : inputDetection.GetCameraMovement().x * keyboardMoveSpeed,
                        inputDetection.inputDeviceType == E_InputDeviceType.gamepad ?
                        inputDetection.GetCameraMovement().y : inputDetection.GetCameraMovement().y * keyboardMoveSpeed);
        //Update rotate value
        //x
        mRotateValue.x += inputDelta.x * rotateSpeed * Time.smoothDeltaTime;
        mRotateValue.x = AngleCorrection(mRotateValue.x);
        //y
        mRotateValue.y += inputDelta.y * rotateSpeed * (invertPitch ? -1 : 1) * Time.smoothDeltaTime;
        mRotateValue.y = AngleCorrection(mRotateValue.y);
        mRotateValue.y = Mathf.Clamp(mRotateValue.y, pitchLimit.x, pitchLimit.y);

        //update yaw
        horizontalQuat = Quaternion.AngleAxis(mRotateValue.x, mYawRotateAxis);

        //apply yaw around up axis
        Vector3 yawedDir = horizontalQuat * mDefaultDir;

        //recompute pitch axis based on yawed direction
        Vector3 flatForward = Vector3.ProjectOnPlane(yawedDir, mYawRotateAxis);

        mPitchRotateAxis = horizontalQuat * Vector3.right;
        //if (flatForward.sqrMagnitude < 0.0001f)
        //    //mPitchRotateAxis = transform.right;
        //    /******************************************/
        //    mPitchRotateAxis = horizontalQuat * Vector3.right; // stable pitch axis - always cam right
        //else
        //    mPitchRotateAxis = Vector3.Cross(mYawRotateAxis, flatForward.normalized);

        //mPitchRotateAxis = Vector3.Cross(mYawRotateAxis, flatForward.normalized);

        verticalQuat = Quaternion.AngleAxis(mRotateValue.y, mPitchRotateAxis);
        //finalDir = horizontalQuat * verticalQuat * mDefaultDir;
        finalDir = verticalQuat * yawedDir;

        from = playerTransform.localToWorldMatrix.MultiplyPoint3x4(offset);

        to = from + finalDir * distance;

        exceptTo = ObstacleProcess(from, to);
        expectDistance = Vector3.Distance(exceptTo, from);

        if (expectDistance < mCurrentDistance)
        {
            mCurrentDistance = expectDistance;
            mDistanceRecoveryDelayCounter = distanceRecoveryDelay;
        }
        else
        {
            if (mDistanceRecoveryDelayCounter > 0f)
                mDistanceRecoveryDelayCounter -= Time.deltaTime;
            else
                mCurrentDistance = Mathf.Lerp(mCurrentDistance, expectDistance, Time.smoothDeltaTime * distanceRecoverySpeed);
        }

        //this.transform.position = from + finalDir * mCurrentDistance;
        movePos = from + finalDir * mCurrentDistance;

        transform.position = Vector3.Lerp(this.transform.position, movePos, Time.deltaTime * moveSpeed);
        Quaternion targetRot = Quaternion.LookRotation(from - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);
        //this.transform.LookAt(from);

        //if (!resetCamPos && inputDelta.magnitude != 0)
        //{
        //    resetCamPos = true;
        //}
        //else if(resetCamPos)
        //{
        //    //transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(finalDir), Time.deltaTime * rotateSpeed);
        //    transform.position = Vector3.Lerp(this.transform.position, movePos, Time.deltaTime * moveSpeed);
        //    this.transform.LookAt(from);
        //}

    }
    #endregion

    #region Sync Rotate Value

    public void SyncRotateValueFromCamera()
    {
        Vector3 pivot = playerTransform.localToWorldMatrix.MultiplyPoint3x4(offset);

        // direction from pivot -> camera 
        Vector3 orbitDir = (transform.position - pivot).normalized;

        //1.Update default yaw reference
        Vector3 planar = Vector3.ProjectOnPlane(orbitDir, mYawRotateAxis);
        if (planar.sqrMagnitude < 0.0001f)
            planar = Vector3.ProjectOnPlane(transform.forward, mYawRotateAxis); // fallback

        mDefaultDir = planar.normalized;

        //2.Reset yaw so the current direction is treated as yaw = 0 (no snapping)
        mRotateValue.x = 0f;

        //3.Sync pitch from current orbit direction
        float pitch = -Mathf.Asin(Vector3.Dot(orbitDir, mYawRotateAxis)) * Mathf.Rad2Deg;
        mRotateValue.y = Mathf.Clamp(pitch, pitchLimit.x, pitchLimit.y);
    }
    #endregion

    #region Angle Correction Range
    /// <summary>
    /// Prevent the angle value from becoming too large by keeping it within the range (-180, 180)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private float AngleCorrection(float angle)
    {
        angle %= 360f;
        //if (angle > 180f)
        //    return mRotateValue.x - 360f;
        //else if (angle < -180)
        //    return mRotateValue.x + 360;
        //return angle;

        if(angle > 180f)
            angle -= 360f;
        else if(angle < -180f)
            angle += 360f;
        return angle;
    }
    #endregion

    #region Collision Detection Method 
    Vector3 ObstacleProcess(Vector3 from, Vector3 to)
    {
        dir = (to - from).normalized;

        // Check if the starting point is already inside an obstacle
        if (Physics.CheckSphere(from, detectorSphereRadius, obstacleMask))
            Debug.Log("Error, Detector radius should be smaller than the object size.");

        // Perform a sphere cast to detect obstacles between 'from' and 'to'
        hit = default(RaycastHit);
        isHit = Physics.SphereCast(new Ray(from, dir), detectorSphereRadius, out hit, distance, obstacleMask);

        if (isHit)
        {
            //Debug.Log("Hit obstacle");
            return hit.point + (-dir * detectorSphereRadius);
        }

        return to;

    }
    #endregion

    #region ConeSightDetection & transition
    public void ConeSightCamMovement()
    {
        if (playerTransform == null) return;
        if (isBelndingToCone) return;

        if (!hasInitialisedConeCam)
        {
            hasInitialisedConeCam = true;

            // keep current yaw (or use player yaw if you want)
            mRotateValue.x = transform.eulerAngles.y;
            mRotateValue.y = Mathf.Clamp(mRotateValue.y, pitchLimitCD.x, pitchLimitCD.y);

            Quaternion initRot = Quaternion.Euler(mRotateValue.y, mRotateValue.x, 0f);
            Vector3 initPos = playerTransform.position + initRot * camOffset;

            transform.position = initPos;
            transform.rotation = initRot;
            return;
        }

        Vector2 rawInput = inputDetection.GetCameraMovement();
        inputDelta = new Vector2(
            inputDetection.inputDeviceType == E_InputDeviceType.gamepad ? rawInput.x : rawInput.x * keyboardMoveSpeed,
            inputDetection.inputDeviceType == E_InputDeviceType.gamepad ? rawInput.y : rawInput.y * keyboardMoveSpeed
        );

        mRotateValue.x += inputDelta.x * rotateSpeed * Time.smoothDeltaTime;
        mRotateValue.y += inputDelta.y * rotateSpeed * Time.smoothDeltaTime;
        mRotateValue.y = Mathf.Clamp(mRotateValue.y, pitchLimitCD.x, pitchLimitCD.y);

        Quaternion camRot = Quaternion.Euler(mRotateValue.y, mRotateValue.x, 0f);

        Vector3 desiredPos = playerTransform.position + camRot * camOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, moveSpeed_coneSight * Time.deltaTime);
        transform.rotation = camRot;

        RotatePlayerToCamera(camRot);
    }

    void RotatePlayerToCamera(Quaternion camRot)
    {
        if(playerTransform == null) return;
        Vector3 forward = camRot * Vector3.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(forward);
            playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRot, moveSpeed_coneSight * Time.deltaTime);
        }
    }

    public void BeginConeTransition(bool keepCurrentYam = true)
    {
        if(playerTransform == null) return;

        if (blendRoutine != null) StopCoroutine(blendRoutine);
        {
            blendRoutine = StartCoroutine(BlendNormalToCone(keepCurrentYam));
        }
    }

    IEnumerator BlendNormalToCone(bool keepCurrentYaw)
    {
        isBelndingToCone = true;

        //start pos 
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        if(keepCurrentYaw)
            mRotateValue.x = this.transform.eulerAngles.y; //keep current view direction
        else
            mRotateValue.x = playerTransform.eulerAngles.y; //face behind player

        mRotateValue.y = Mathf.Clamp(mRotateValue.y, pitchLimitCD.x, pitchLimitCD.y);

        //target cone pos & rot
        Quaternion endRot = Quaternion.Euler(mRotateValue.y, mRotateValue.x, 0f);
        Vector3 endPos = playerTransform.position + endRot * camOffset;

        float t = 0f;
        while(t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, normalToConeBlendTime);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        //snap exact at end
        transform.position = endPos;
        transform.rotation = endRot;

        hasInitialisedConeCam = true;
        isBelndingToCone = false;
    }
    #endregion
}

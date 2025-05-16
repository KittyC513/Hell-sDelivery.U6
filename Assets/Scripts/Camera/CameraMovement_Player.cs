//Comment out `undef` or `def Newwork` to switch between script versions
// need to be set on PlayerController as well
#define Network
#undef Network

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

    [Header("Camera Variables")]
    Vector3 mDefaultDir;
    [Tooltip("rotate value")]
    Vector3 mRotateValue;
    //横向:偏航角 up_down rotate
    Vector3 mPitchRotateAxis;
    //纵向:俯仰角 left_right rotate
    Vector3 mYawRotateAxis;
    public float distance = 4;
    public float rotateSpeed = 120f;
    public Vector3 offset = new Vector3(0f, 1.5f, 0f);

    public bool invertPitch;
    public Vector2 pitchLimit = new Vector2(-40f, 70f);

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
    public Transform defaultPos;
    public bool resetPos = false;

    public CameraManager cameraManager;
    public bool isResetPos = true;
    private Vector3 camInput;

    private void Start()
    {
        // get y axis
        var upAxis = -Physics.gravity.normalized;
        //set cam default regarding player's position
        mDefaultDir = Vector3.ProjectOnPlane(transform.position - playerTransform.position, upAxis).normalized;
        //Initial yam and pitch axis
        mYawRotateAxis = upAxis;
        mPitchRotateAxis = Vector3.Cross(upAxis, Vector3.ProjectOnPlane(transform.forward, upAxis));

#if Network

        if (!IsOwner)
            this.gameObject.SetActive(false);
#endif
    }
    private void OnEnable()
    {
        //// get y axis
        //var upAxis = -Physics.gravity.normalized;
        ////set cam default regarding player's position
        //mDefaultDir = Vector3.ProjectOnPlane(transform.position - playerTransform.position, upAxis).normalized;
        ////Initial yam and pitch axis
        //mYawRotateAxis = upAxis;
        //mPitchRotateAxis = Vector3.Cross(upAxis, Vector3.ProjectOnPlane(transform.forward, upAxis));

        //Reset function 
        mRotateValue.x = playerTransform.eulerAngles.y; // face behind player
        mRotateValue.y = Mathf.Clamp(distance, pitchLimit.x, pitchLimit.y); // default pitch

        this.transform.position = cameraManager.lockCam.transform.position;
        //ResetPos(defaultPos);
    }
    private void Update()
    {

    }


    void LateUpdate()
    {
        CameraMovement();

        //if (!resetPos)
        //{
        //    resetPos = true;
        //    //ResetPos(defaultPos);
        //}
        //else
        //{      
        //    CameraMovement();
        //}

    }


    #region Player Camera movement(Base)
    void CameraMovement()
    {

        //get input value
        inputDelta = new Vector2(inputDetection.inputDeviceType == E_InputDeviceType.Gamepad ?
                        inputDetection.GetCameraMovement().x : inputDetection.GetCameraMovement().x * keyboardMoveSpeed,
                        inputDetection.inputDeviceType == E_InputDeviceType.Gamepad ?
                        inputDetection.GetCameraMovement().y : inputDetection.GetCameraMovement().y * keyboardMoveSpeed);
        //Update rotate value
        //x
        mRotateValue.x += inputDelta.x * rotateSpeed * Time.smoothDeltaTime;
        mRotateValue.x = AngleCorrection(mRotateValue.x);
        //y
        mRotateValue.y += inputDelta.y * rotateSpeed * (invertPitch ? -1 : 1) * Time.smoothDeltaTime;
        mRotateValue.y = AngleCorrection(mRotateValue.y);
        mRotateValue.y = Mathf.Clamp(mRotateValue.y, pitchLimit.x, pitchLimit.y);

        //update yam
        horizontalQuat = Quaternion.AngleAxis(mRotateValue.x, mYawRotateAxis);
        verticalQuat = Quaternion.AngleAxis(mRotateValue.y, mPitchRotateAxis);
        finalDir = horizontalQuat * verticalQuat * mDefaultDir;

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
        //transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(finalDir), Time.deltaTime * rotateSpeed);
        this.transform.LookAt(from);


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
        if (angle > 180f)
            return mRotateValue.x - 360f;
        else if (angle < -180)
            return mRotateValue.x + 360;
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
            Debug.Log("Hit obstacle");
            return hit.point + (-dir * detectorSphereRadius);
        }

        return to;

    }
    #endregion
}

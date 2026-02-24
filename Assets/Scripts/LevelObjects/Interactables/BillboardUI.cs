using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class BillboardUI : MonoBehaviour
{
    protected Camera p1Cam;
    protected Camera p2Cam;

    [SerializeField] protected Canvas billboardPrefab;
    //[SerializeField] private Transform positionFollow;

    [SerializeField] protected bool player1Active = false;
    [SerializeField] protected bool player2Active = false;

    [SerializeField] protected bool globalIcons = false;

    [SerializeField] protected float yOffset = 0.5f;

    [SerializeField] private bool applySine = true;
    [SerializeField] private float frequency = 2.5f;
    [SerializeField] private float amplitude = 0.15f;
    public bool active = true;

    [Space, Header("Lock Rotations")]
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;
    private Vector3 lockVector;

    private Vector3 pos;
    protected Vector3 startPos;
    private Vector3 changeInPos;

    [HideInInspector] public List<GameObject> images;


    public delegate void OnInitialize();
    public OnInitialize onInitialize;

    protected virtual void SetupBillboard()
    {
        p1Cam = GameManager.instance.cam_p1;
        p2Cam = GameManager.instance.cam_p2;
        images = new List<GameObject>();

        images.Add(Instantiate(billboardPrefab, this.transform.position, Quaternion.identity, this.transform).gameObject);
        images.Add(Instantiate(billboardPrefab, this.transform.position, Quaternion.identity, this.transform).gameObject);

        //0 = player 1
        //1 = player 2

        if (!globalIcons)
        {
            images[0].layer = LayerMask.NameToLayer("UI_P2Ignore");
            images[1].layer = LayerMask.NameToLayer("UI_P1Ignore");
        }
        else
        {
            images[0].GetComponentInChildren<Image>().color = Color.red;
            images[1].GetComponentInChildren<Image>().color = Color.blue;
        }
        onInitialize?.Invoke();
    }

    private void Start()
    {
        SetupBillboard();
    }

    public void EnableAllIcons()
    {
        player1Active = true;
        player2Active = true; 
    }

    public void DisableAllIcons()
    {
        player1Active = false;
        player2Active = false; 
    }

    

    public void ShowIconToPlayer(bool show, int player)
    {
        if (player == 1)
        {
            player1Active = show;
        }
        else
        {
            player2Active = show;   
        }
    }

    public bool IsShowingToPlayer(int player)
    {
        if (player == 1)
        {
            return player1Active;
        }
        else
        {
            return player2Active;
        }
    }

    private void Update()
    {
       ToggleUI();

      
    }

    protected virtual void ToggleUI()
    {
        //update the unaltered position
        startPos = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);

        //set the camera if its still null
        if (p1Cam == null) p1Cam = GameManager.instance.cam_p1;
        if (p2Cam == null) p2Cam = GameManager.instance.cam_p2;

        if(images[0] != null && images[1] != null)
        {
            //show the ui if the player is in range
            if (player1Active && p1Cam != null && active)
            {
                images[0].SetActive(true);
                BillboardToCamera(p1Cam, images[0]);
            }
            else
            {
                images[0].SetActive(false);
            }

            if (player2Active && p2Cam != null && active)
            {
                images[1].SetActive(true);
                BillboardToCamera(p2Cam, images[1]);
            }
            else
            {
                images[1].SetActive(false);
            }
        }
    }



    protected void BillboardToCamera(Camera cam, GameObject img)
    {
        //apply the y offset from the sin wave
        float yPos = amplitude * Mathf.Sin(Time.time * frequency);
        pos = new Vector3(startPos.x, startPos.y + yPos, startPos.z);

        img.transform.position = pos;

        //img.transform.LookAt(cam.transform.position);
        
        LookatCamera(cam, img);
    }

    private void LookatCamera(Camera cam, GameObject img)
    {
        //get the direction to the camera
        Vector3 direction = (cam.transform.position - img.transform.position).normalized;
        //get a vector based on the locked angles
        lockVector = new Vector3(lockX == true ? 0 : 1, lockY == true ? 0 : 1, lockZ == true ? 0 : 1);
        direction = new Vector3(direction.x * lockVector.x, direction.y * lockVector.y, direction.z * lockVector.z);

        //get a quaternion rotation using the camera's up vector
        Quaternion targetDirection = Quaternion.LookRotation(direction, cam.transform.up);
        //rotate towards the target direction
        img.transform.rotation = Quaternion.Slerp(img.transform.rotation, targetDirection, 100 * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z), new Vector3(0.25f, 0.25f + amplitude, 0.25f));
    }

}


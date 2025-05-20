using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class BombItem : MonoBehaviour
{
    public ItemHandler iHandler;
    public PlayerLockOn playerLockOn;

    public Transform targetPos;
    public float dropSpeed = 5f;

    public Collider worldCollider;
    public Rigidbody rb;

    public GameObject bombPrefab;
    public bool isSpawned = false;

    [Header("Bomb")]
    public Vector3 offset; //to replace bombs
    public int maxBombs = 1;
    public int numOfBombs = 0;

    //cooldown setting
    public float cdSpawn = 0.3f;
    private float timer = 0;
    public bool canStartTimer = false;


    public List<BombMovement> bombsList = new List<BombMovement>();

    private void Awake()
    {
        iHandler = GetComponent<ItemHandler>();
        if (iHandler == null) Debug.Log(this.ToString() + " needs an itemHandler attached on the gameObject " + this.gameObject.name); 
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        iHandler.onItemTrigger += ThrowBomb;
    }

    private void OnDisable()
    {
        iHandler.onItemTrigger -= ThrowBomb; 
    }

    private void ThrowBomb()
    {
        if(numOfBombs < maxBombs)
        {
            if (!isSpawned && timer >= cdSpawn)
            {
                // while throwing bomb, generating a bomb object and reset the position to player's pos
              
                GameObject bombObj = Instantiate(bombPrefab);
                bombObj.transform.position = this.transform.position;
                //bombObj.transform.rotation = iHandler.iControl.transform.rotation;
                if (Vector3.Distance(bombObj.transform.position, this.transform.position) < 0.1f)
                    bombObj.transform.parent = iHandler.iControl.transform;
                worldCollider.gameObject.SetActive(false);

                bombsList.Add(bombObj.GetComponent<BombMovement>());
                numOfBombs++;
                isSpawned = true;
                timer = 0;
            }

        }

    }
    public void Update()
    {
        //cooldown timer starts
        if (canStartTimer)
        {
            if (timer < cdSpawn)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = cdSpawn;
            }
        }

        //When the item is equipped, the item will follow player's pos
        if (iHandler.equipped)
        {
            /**************************************/
            /**************************************/
            //Set object as a child of a player 


            //parentObj = iHandler.iControl.gameObject.transform.parent;
            //this.transform.position = parentObj.transform.position;

            if (Vector3.Distance(this.transform.position, iHandler.iControl.transform.position + offset) < 0.1f)
                this.transform.SetParent(iHandler.iControl.transform);
            else
                this.transform.position = iHandler.iControl.transform.position + offset;
            //Debug.Log("Parent");

            /**************************************/
            /**************************************/
        }
    }
}

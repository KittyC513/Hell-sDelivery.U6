using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class BombItem : ItemBase
{
    //public ItemHandler iHandler;

    public Transform targetPos;
    public float dropSpeed = 5f;

    public GameObject bombPrefab;

    [Header("Bomb")]
    public Vector3 offset; //to replace bombs
    public int maxBombs = 1;
    public int numOfBombs = 0;

    //cooldown setting
    public float cdSpawn = 0.3f;
    public float timer = 0;

    public List<BombMovement> bombsList = new List<BombMovement>();

    //protected override void Awake()
    //{
    //    base.Awake();
    //    //itemHandler = GetComponent<ItemHandler>();
    //    //iHandler = GetComponent<ItemHandler>();
    //    //if (iHandler == null) Debug.Log(this.ToString() + " needs an itemHandler attached on the gameObject " + this.gameObject.name); 
    //}

    //In Start method
    //public override void Initialize()
    //{
    //    //itemHandler.onItemTrigger += ThrowBomb;
    //    print("BombItem initialized and ready to use");
    //}

    //private void OnDisable()
    //{
    //    //itemHandler.onItemTrigger -= ThrowBomb; 
    //}

    public override void UseFunction()
    {
        ThrowBomb();
        print("Bomb is thrown");
    }
    private void ThrowBomb()
    {
        if(numOfBombs < maxBombs)
        {
            if (timer >= cdSpawn)
            {
                print(cdSpawn + " seconds cooldown is over, you can throw a bomb now");
                // while throwing bomb, generating a bomb object and reset the position to player's pos

                GameObject bombObj = Instantiate(bombPrefab);
                bombObj.transform.position = this.transform.position;
                //bombObj.transform.rotation = iHandler.iControl.transform.rotation;
                //if (Vector3.Distance(bombObj.transform.position, this.transform.position) < 0.1f)
                BombMovement bombMovement = bombObj.GetComponent<BombMovement>();
                bombMovement.playerLockOn = this.transform.parent.GetComponent<PlayerLockOn>();
                
                if (bombMovement.playerLockOn.lockTarget != null)
                    bombMovement.targetPos = bombMovement.playerLockOn.lockTarget.transform;

                bombsList.Add(bombMovement);
                numOfBombs++;
                timer = 0;
            }

        }

    }
    public void FixedUpdate()
    {
        //cooldown timer starts
        if (timer < cdSpawn)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = cdSpawn;
        }
        

        ////When the item is equipped, the item will follow player's pos
        //if (itemHandler.equipped)
        //{
        //    /**************************************/
        //    /**************************************/
        //    //Set object as a child of a player 


        //    //parentObj = iHandler.iControl.gameObject.transform.parent;
        //    //this.transform.position = parentObj.transform.position;

        //    if (Vector3.Distance(this.transform.position, itemHandler.iControl.transform.position + offset) < 0.1f)
        //        this.transform.SetParent(itemHandler.iControl.transform);
        //    else
        //        this.transform.position = itemHandler.iControl.transform.position + offset;
        //    //Debug.Log("Parent");

        //    /**************************************/
        //    /**************************************/
        //}

        //if (!itemHandler.equipped && (worldCollider.enabled == false || itemHandler.rb.useGravity == false))
        //{
        //    worldCollider.enabled = true;
        //    itemHandler.rb.useGravity = true;
        //}

    }
}

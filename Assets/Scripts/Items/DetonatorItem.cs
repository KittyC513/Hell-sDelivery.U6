using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DetonatorItem : MonoBehaviour
{
    public ItemHandler iHandler;
    public BombItem bombItem;
    public Collider worldCollider;
    public Vector3 offset;

    public List<BombMovement> list;
   
    //cooldown setting
    public float cdTime = 0.3f;
    private float timer = 0;
    public bool canStartTimer = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        iHandler = GetComponent<ItemHandler>();
    }
    void Start()
    {
        iHandler.onItemTrigger += Ignite;
        timer = 0;
    }

    private void OnDisable()
    {
        iHandler.onItemTrigger -= Ignite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Ignite()
    {
        if(bombItem.bombsList.Count > 0 && timer >= cdTime)
        {
            for (int i = 0; i <= bombItem.bombsList.Count - 1; i++)
            {

                bombItem.bombsList[i].ApplyExplosionForce();
                print("Trigger" + bombItem.bombsList[i].name);

            }

            bombItem.bombsList.Clear();
            bombItem.numOfBombs = 0;
        }

    }

    private void FixedUpdate()
    {
        if (canStartTimer)
        {
            if (timer >= cdTime)
                timer = cdTime;
            else
                timer += Time.deltaTime;
        }
        if (iHandler.equipped)
        {
            if (worldCollider.enabled == true)
                    worldCollider.enabled = false;
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

        if (!iHandler.equipped && (worldCollider.enabled == false || iHandler.rb.useGravity == false))
        {
            worldCollider.enabled = true;
            iHandler.rb.useGravity = true;
        }


    }
}

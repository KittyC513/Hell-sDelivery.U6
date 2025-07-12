using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DetonatorItem : ItemBase
{
    //public ItemHandler iHandler;
    public BombItem bombItem;
    public Vector3 offset;

    public List<BombMovement> list;
   
    //cooldown setting
    public float cdSpawn = 0.3f;
    private float timer = 0;

    private bool canStartTimer = false;


    //protected override void Awake()
    //{
    //    base.Awake();
    //}
    //private void OnDisable()
    //{
    //    //iHandler.onItemTrigger -= Ignite;
    //}


    public void Ignite()
    {
        if(bombItem.bombsList.Count > 0 && timer >= cdSpawn)
        {
            for (int i = 0; i <= bombItem.bombsList.Count - 1; i++)
            {
                bombItem.bombsList[i].ApplyExplosionForce();
            }

            bombItem.bombsList.Clear();
            bombItem.numOfBombs = 0;
            timer = 0;
            canStartTimer = false;
        }

    }

    private void FixedUpdate()
    {
        if (timer < cdSpawn && canStartTimer)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = cdSpawn;
        }

        if (isOnUse)
        {
            if (!inputDetection.crouchPressed && timer == 0)
                canStartTimer = true;
        }

    }

    public override void UseFunction()
    {
        Ignite();
    }
}

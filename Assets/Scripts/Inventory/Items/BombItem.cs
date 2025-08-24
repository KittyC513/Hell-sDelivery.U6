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

    public PlayerStateMachine pStateMachine;
    public bool validatedUse = false;


    public override void UseFunction()
    {
        if (validatedUse)
        {
            ThrowBomb();
            print("Bomb is thrown");
        }
    }
    private void ThrowBomb()
    {
        if (numOfBombs < maxBombs)
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
     
    }

    protected override void Update()
    {
        base.Update();
        ValidatedCheck();
    }
    private void ValidatedCheck()
    {
        if (isOnUse)
        {
            if(pStateMachine != null)
            {
                if (pStateMachine.showCurrentState == PlayerStateMachine.PlayerStates.freeFall)
                    validatedUse = false;
                else
                    validatedUse = true;
            }
            else
            {
                pStateMachine = this.transform.parent.GetComponent<PlayerStateMachine>();
            }
        }
        else
        {
            validatedUse = false;
            pStateMachine = null;
        }
    }
}

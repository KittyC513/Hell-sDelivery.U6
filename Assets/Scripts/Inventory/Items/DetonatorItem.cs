using NUnit.Framework;
using System.Collections;
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
    [SerializeField] private SoundEffectPlayer sfxPlayer;


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
        ////detonate all bombs in the bomb list
        //if (bombItem.bombsList.Count > 0 && timer >= cdSpawn)
        //{
        //    for (int i = 0; i <= bombItem.bombsList.Count - 1; i++)
        //    {
        //        bombItem.bombsList[i].ApplyExplosionForce();
        //    }

        //    bombItem.bombsList.Clear();
        //    bombItem.numOfBombs = 0;
        //    timer = 0;
        //    canStartTimer = false;
        //}

        //detonate all bombs in cone sight detection
        if (timer >= cdSpawn && playerLockOn.visibleTargets.Count > 0)
        {
            StartCoroutine(StartIgnite());
            
        }

        if (timer >= cdSpawn)
        {
            sfxPlayer.PlaySoundEffect("ObjectBank1", "DetonatorPing");
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

    IEnumerator StartIgnite()
    {
        var snapshpt = playerLockOn.visibleTargets.ToArray();

        foreach (Transform bomb in snapshpt)
        {
            BombMovement bombMove = bomb.GetComponent<BombMovement>();
            if (bombMove != null)
            {
                StartCoroutine(bombMove.ApplyExplosionForce());
                bombItem.bombsList.Remove(bombMove);
                playerLockOn.visibleTargets.Remove(bomb);
                bombItem.numOfBombs--;
            }
        }
        //for (int i = 0; i <= playerLockOn.visibleTargets.Count - 1; i++)
        //{
        //    BombMovement bomb = playerLockOn.visibleTargets[i].GetComponent<BombMovement>();
        //    print("GainBomb");

        //    if (bomb != null)
        //    {
        //        bomb.ApplyExplosionForce();
        //        bombItem.bombsList.Remove(bomb);
        //        playerLockOn.visibleTargets.RemoveAt(i);
        //        bombItem.numOfBombs--;
        //    }
        //}
        timer = 0;
        canStartTimer = false;

        yield return null;
    }
}

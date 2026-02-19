using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CombatSequence : MonoBehaviour
{
    [SerializeField] public UnityEvent onSequenceEnd;
    [SerializeField] public UnityEvent onSequenceStart;
    [SerializeField] public bool shouldSequenceActivate = true; //this value can be changed externally to activate the combat zone, to avoid it being active before enemies spawn
    private bool sequenceActive = false;
    private int playersActive;
    [SerializeField] private LayerMask playerLayers;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Vector3 areaSize;
    private Collider[] players;

    private Collider[] enemies;

    private void Start()
    {
        players = new Collider[0];
        enemies = new Collider[0];
    }
    private void Update()
    {
    
        if (shouldSequenceActivate)
        {
            DetectPlayers();
        }
        

        if (sequenceActive)
        {
            DetectEnemies();
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, areaSize);
    }

    private void EndSequence()
    {
        if (sequenceActive)
        {
            sequenceActive = false;
        }
        print("Combat Sequence Ended");
        onSequenceEnd.Invoke();
    }

    private void StartSquence()
    {
        if (!sequenceActive)
        {
            sequenceActive = true;
            
        }
    
        onSequenceStart.Invoke();
    }

    private void DetectPlayers()
    {
        players = Physics.OverlapBox(transform.position, areaSize, Quaternion.identity, playerLayers);
        
        if (players.Length > 3 && !sequenceActive)
        {
            StartSquence();
        }
    }

    private void DetectEnemies()
    {
        enemies = Physics.OverlapBox(transform.position, areaSize, Quaternion.identity, enemyLayer);

        if (enemies.Length < 1 && sequenceActive)
        {
            EndSequence();
        }
    }

}

using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CombatSequence : MonoBehaviour
{
    [SerializeField] public UnityEvent onSequenceEnd;
    [SerializeField] public UnityEvent onSequenceStart;
    private bool sequenceActive = false;
    private int playersActive;
    [SerializeField] private LayerMask playerLayers;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float areaRadius;
    private Collider[] players;

    private Collider[] enemies;

    private void Start()
    {
        players = new Collider[0];
        enemies = new Collider[0];
    }
    private void Update()
    {
        DetectPlayers();
        DetectEnemies();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, areaRadius);
    }

    private void EndSequence()
    {
        if (sequenceActive)
        {
            sequenceActive = false;
        }

        onSequenceEnd.Invoke();
    }

    private void StartSquence()
    {
        if (!sequenceActive)
        {
            sequenceActive = true;
            print("Combat Sequence Started");
        }
    
        onSequenceStart.Invoke();
    }

    private void DetectPlayers()
    {
        players = Physics.OverlapSphere(transform.position, areaRadius, playerLayers);
        
        if (players.Length > 1 && !sequenceActive)
        {
            StartSquence();
        }
    }

    private void DetectEnemies()
    {
        enemies = Physics.OverlapSphere(transform.position, areaRadius, enemyLayer);

        if (enemies.Length < 1 && sequenceActive)
        {
            EndSequence();
        }
    }

}

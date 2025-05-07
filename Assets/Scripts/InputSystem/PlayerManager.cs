using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Player spawn point manager
/// </summary>
public class PlayerManager : MonoBehaviour
{
    private List<PlayerInput> players = new List<PlayerInput>();

    [SerializeField]
    private List<Transform> startPoints;
    [SerializeField]
    private List<LayerMask> playerLayers;

    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        playerInputManager = FindFirstObjectByType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        //add player when join button has been pressed
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        //delete player count when it's disabled
        playerInputManager.onPlayerJoined -= AddPlayer;
    }

    public void AddPlayer(PlayerInput player)
    {
        players.Add(player);

        //Set player's(whole player prefab) start position to the spawn point
        Transform playerParent = player.transform.parent;
        playerParent.position = startPoints[players.Count - 1].position;

        //convert layer mask to an int
        int layerToAdd = (int)Mathf.Log(playerLayers[players.Count - 1].value, 2);

        //set the layer
        //for future reference
        player.gameObject.layer = layerToAdd;
    }

}

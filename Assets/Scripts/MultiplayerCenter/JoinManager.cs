using System.Linq;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Users;

public class JoinManager : MonoBehaviour
{

    public GameObject playerPrefab;

    public PlayerInputManager inputManager;

    bool isSpawned = false;

    // Update is called once per frame
    void Update()
    {
        SpawnCheck();
    }

    public void SpawnCheck()
    {
        // Check if the player is already connected or if the maximum number of players has been reached
        if (!NetworkManager.Singleton.IsConnectedClient || isSpawned) return;


        if(Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame || Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            isSpawned = true;

            if (NetworkManager.Singleton.IsHost)
            {
                
                //Spawn player
                SpawnPlayer(NetworkManager.Singleton.LocalClientId);

                Debug.Log("PlayerCount: " + inputManager.playerCount);

            }
            else
            {
                RequestSpawnServerRpc();

                Debug.Log("PlayerCount: " + inputManager.playerCount);
            }

        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnServerRpc(ServerRpcParams rpcParams = default)
    {
        SpawnPlayer(rpcParams.Receive.SenderClientId);
        Debug.Log($"[Client] LocalClientId: {NetworkManager.Singleton.LocalClientId}");

    }

    private void SpawnPlayer(ulong clientId)
    {
        GameObject player = Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        Debug.Log($"[Server] Spawning for client: {clientId}");



    }

    #region Pair device

    #endregion

}

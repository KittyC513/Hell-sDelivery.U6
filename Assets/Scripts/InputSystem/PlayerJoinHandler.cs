using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinHandler : MonoBehaviour
{
    private NetworkObject netObj;
    public GameObject playerPrefab;
    private bool isSpawned = false;

    private void Awake()
    {
        netObj = GetComponent<NetworkObject>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isSpawned) return;

        if (NetworkManager.Singleton.IsServer)
        {
            netObj.Spawn();
            Debug.Log("Server Spawned");
            isSpawned = true;
        }
    }

    public void OnJoinPressed()
    {
        if(NetworkManager.Singleton.IsClient)
        {
            // Request the server to spawn the player object
            RequestJoinServerRpc();
            Debug.Log("Join button pressed, requesting server to spawn player.");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestJoinServerRpc(ServerRpcParams rpcParams = default)
    {
        //netObj.SpawnAsPlayerObject(rpcParams.Receive.SenderClientId);
        ulong clientId = rpcParams.Receive.SenderClientId;

        GameObject player = Instantiate(playerPrefab, GetSpawnPos(), Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

        isSpawned = true;
    }


    private Vector3 GetSpawnPos()
    {
        // Random spawn position or you could use spawn points
        return new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
    }

}



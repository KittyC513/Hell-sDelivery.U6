using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Diagnostics;

public class JoinManager : MonoBehaviour
{
    private bool hasJoined = false;
    public GameObject playerPrafab;

    private void Update()
    {
        if (hasJoined || !NetworkManager.Singleton.IsClient) return;

        if(Keyboard.current.anyKey.wasPressedThisFrame || Gamepad.current?.buttonSouth.wasPressedThisFrame == true)
        {
            hasJoined = true;
            SubmitJoinRequestServerRpc();
        }

    }

    [ServerRpc(RequireOwnership = false)]
    void SubmitJoinRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        // Instantiate and spawn the player on the server
        GameObject player = Instantiate(playerPrafab, GetSpawnPos(), Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    Vector3 GetSpawnPos()
    {
        return new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
    }


}

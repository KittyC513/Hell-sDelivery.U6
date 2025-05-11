using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    float moveSpeed = 5f;

    public struct MyCustomData 
    {
        
    }

    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (int pervisouValue, int newValue) =>
        {
            Debug.Log(OwnerClientId + "_Random Number: " + randomNumber.Value);
        };
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        if (!IsOwner) return;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            randomNumber.Value = Random.Range(0, 101);
        }
        Vector3 moveDir = new Vector3(0,0,0);

        if(Input.GetKey(KeyCode.W))
        {
            moveDir.z += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir.z -= 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir.x += 1;
        }

        transform.position += moveDir * moveSpeed * Time.deltaTime;

    }
}

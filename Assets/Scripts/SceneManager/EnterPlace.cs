using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class EnterPlace : MonoBehaviour
{
    public string enterPlaceName;

    public RaycastHit[] hits;
    public Vector3 halfSize = new Vector3(5, 5, 5);
    public float maxDistance = 5f;

    public Transform enterPlace;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //DetectEnter();
    }

    private void DetectEnter()
    {
        //when players are in bark range, the text UI will trigger, otherwise, it will stay bark UI
        hits = Physics.BoxCastAll(this.transform.position, halfSize, this.transform.forward, Quaternion.identity, maxDistance,
                                                1 << LayerMask.NameToLayer("Player1") | 1 << LayerMask.NameToLayer("Player2"));
        // group the hits when it comes from the same object with multiple colliders
        hits = hits.GroupBy(h => h.collider.gameObject).Select(g => g.First()).ToArray();
        print(hits.Length);

        if (hits.Length == 2)
            print("Enter" + enterPlaceName);
        else
            print("waiting for the other player");

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(this.transform.position, 2 * halfSize);
    }

    #region Split-screen
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player1") || other.gameObject.layer == LayerMask.NameToLayer("Player2"))
        {
            other.GetComponent<PlayerLockOn>().CameraManager.alleywayCam.GetComponent<CameraMovement_Alleyway>().isTransitioning = true;
            other.gameObject.transform.position = enterPlace.position;
        }
    }
    #endregion
}

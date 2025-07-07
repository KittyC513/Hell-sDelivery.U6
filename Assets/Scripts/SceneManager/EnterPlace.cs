using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterPlace : MonoBehaviour
{
    public string enterPlaceName;

    public RaycastHit[] hits;
    public Vector3 halfSize = new Vector3(5, 5, 5);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectEnter();
    }

    private void DetectEnter()
    {
        //when players are in bark range, the text UI will trigger, otherwise, it will stay bark UI
        hits = Physics.BoxCastAll(this.transform.position, halfSize, this.transform.forward, Quaternion.identity, 0f,
                                                1 << LayerMask.NameToLayer("Player1") | 1 << LayerMask.NameToLayer("Player2") | 1 << LayerMask.NameToLayer("Invisible_Player1") | 1 << LayerMask.NameToLayer("Invisible_Player2"));
        // group the hits when it comes from the same object with multiple colliders
        hits = hits.GroupBy(h => h.collider.gameObject).Select(g => g.First()).ToArray();

        if (hits.Length == 2)
        {
            SceneManager.LoadScene(enterPlaceName);
            print("Enter" + enterPlaceName);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(this.transform.position, 2 * halfSize);
    }
}

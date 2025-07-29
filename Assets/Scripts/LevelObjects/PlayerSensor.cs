using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    //[HideInInspector]
    public bool active = false;
    [SerializeField] private float radius = 1;
    [SerializeField] private LayerMask playerLayer;
    private Collider[] colliders;

    private void Start()
    {
        colliders = new Collider[2];
    }
    private void FixedUpdate()
    {
        DetectPlayers();
    }
    private void DetectPlayers()
    {
        int activeNum = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders, playerLayer);
        
        if (activeNum > 0)
        {
            active = true;
        }
        else
        {
            active = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

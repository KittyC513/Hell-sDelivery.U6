using UnityEngine;

public class ApplyExplosionForce : MonoBehaviour
{
    private Rigidbody rb;
    public float force = 10f;
    public float radius = 5f;
    public Transform bomb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        BombExplosion();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Initiate Bomb")]
    private void BombExplosion()
    {
        rb.AddExplosionForce(force, bomb.position, radius);
        print("Boom!");
    }
}

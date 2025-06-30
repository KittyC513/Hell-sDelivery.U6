using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private GameObject[] detachables; 

    public void DetachObjects()
    {
        for (int i = 0; i < detachables.Length; i++)
        {
            Rigidbody rb = detachables[i].GetComponent<Rigidbody>();
            Collider col = detachables[i].GetComponent<Collider>();

            detachables[i].transform.SetParent(null);
            if (rb != null) rb.isKinematic = false;
            if (col != null) col.enabled = true;
        }
    }
}

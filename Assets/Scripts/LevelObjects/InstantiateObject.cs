using UnityEngine;

public class InstantiateObject : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private Transform spawnPos;
    
    public void SpawnObject()
    {
        if (objectToSpawn != null)
        {
            Instantiate(objectToSpawn, spawnPos.position, Quaternion.identity);
        }
    }
}

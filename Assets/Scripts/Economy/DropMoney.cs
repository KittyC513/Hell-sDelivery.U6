using Unity.Mathematics;
using UnityEngine;


public class DropMoney : MonoBehaviour
{
    //take in multiple prefabs for different values of money


    //drop at random directions with a random force between 2 values in both x and y directions
    [SerializeField] private float2 forceRangeX = new float2(1.5f, 4);
    [SerializeField] private float2 forceRangeY = new float2(4, 8);

    [SerializeField] private MoneyCounts[] moneyCount;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnCurrency();
        }
    }
    public void SpawnCurrency()
    {
        for (int i = 0; i < moneyCount.Length; i++)
        {
            for(int j = 0; j < moneyCount[i].amount; j++)
            {
                GameObject temp = Instantiate(moneyCount[i].moneyPrefab, transform.position, Quaternion.identity);
                Rigidbody tempRB = temp.GetComponent<Rigidbody>();

                if (tempRB != null)
                {
                    float randomX = UnityEngine.Random.Range(forceRangeX.x, forceRangeX.y);
                    float randomZ = UnityEngine.Random.Range(forceRangeX.x, forceRangeX.y);
                    float randomY = UnityEngine.Random.Range(forceRangeY.x, forceRangeY.y);
                    Vector3 randomDir = GetRandomDirection().normalized;
                    Debug.Log(randomDir);
                    Vector3 velocityDir = new Vector3(randomDir.x * randomX, 1 * randomY, randomDir.z * randomZ);
                    Debug.Log(velocityDir);
                    tempRB.AddForce(velocityDir, ForceMode.Impulse);
                }
            }
        }
    }

    public Vector3 GetRandomDirection()
    {
        return (UnityEngine.Random.insideUnitSphere).normalized; ;
    }
}


[System.Serializable]
public struct MoneyCounts
{
    public GameObject moneyPrefab; //the object to spawn
    public float amount; //how many of this object to spawn
}

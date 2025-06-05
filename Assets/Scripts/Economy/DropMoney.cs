using Unity.Mathematics;
using UnityEngine;


public class DropMoney : MonoBehaviour
{
    //take in multiple prefabs for different values of money


    //drop at random directions with a random force between 2 values in both x and y directions
    [SerializeField] private float2 forceRangeX = new float2(1.5f, 4);
    [SerializeField] private float2 forceRangeY = new float2(4, 8);

    //a simple struct that combines a prefab and amount of that prefab to drop
    [SerializeField] private MoneyCounts[] moneyCount;

    public void Update()
    {
        //a debug function to test the dropping of money
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    SpawnCurrency();
        //}
    }
    public void SpawnCurrency()
    {
        for (int i = 0; i < moneyCount.Length; i++)
        {
            for(int j = 0; j < moneyCount[i].amount; j++)
            {
                //spawn a money prefab and get its rigidbody
                GameObject temp = Instantiate(moneyCount[i].moneyPrefab, transform.position, Quaternion.identity);
                Rigidbody tempRB = temp.GetComponent<Rigidbody>();

                //if the rigidbody is not null
                if (tempRB != null)
                {
                    //get random values for the x, y and z
                    float randomX = UnityEngine.Random.Range(forceRangeX.x, forceRangeX.y);
                    float randomZ = UnityEngine.Random.Range(forceRangeX.x, forceRangeX.y);
                    float randomY = UnityEngine.Random.Range(forceRangeY.x, forceRangeY.y);

                    //get a random direction 
                    Vector3 randomDir = GetRandomDirection().normalized;
                    
                    //combine the random values with the random directions
                    Vector3 velocityDir = new Vector3(randomDir.x * randomX, 1 * randomY, randomDir.z * randomZ);
                    
                    //apply to the rigidbody of each object
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

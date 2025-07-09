using UnityEngine;

public class EnemySpawnControl : MonoBehaviour
{
    public GameObject hellHoundPrefab;
    public float spawnTime = 3f;
    public float timer = 0f;
    public int spawnCount = 0;
    public int maxCount = 7;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Spawn(hellHoundPrefab);
    }

    void Spawn(GameObject spawnObj)
    {
        if (spawnCount < maxCount)
        {
            timer += Time.deltaTime;
            //print(timer);

            if(timer >= spawnTime)
            {
                Instantiate(spawnObj,this.transform);
                spawnCount += 1;
                //print(spawnCount);
                timer = 0;
            }
        }
        else
        {
            timer = 0f;
        }

    }
}

using UnityEngine;

public class CarjackMinigameManager : MonoBehaviour
{
    
    [SerializeField] private GameObject carPrefab;
    //the space where cars should be placed
    [SerializeField] private Vector2 screenSize;
    //the amount of cars to be placed x is horizontal and y is vertical
    [SerializeField] private Vector2 carCount;

    [SerializeField] private Transform startPos;

    private void Start()
    {
        //spawn cars across the x
        for (int i = 0; i < carCount.x; i++)
        {
            float percent = (screenSize.x / (carCount.x - 1));
            Vector3 position = new Vector3(startPos.position.x + (i * percent), startPos.position.y, startPos.position.z);

            Instantiate(carPrefab, position, Quaternion.identity);

            //for each x space spawn cars up and down
            for (int y = 1; y < carCount.y; y++)
            {
                float percentY = (screenSize.y / (carCount.y - 1));
                Vector3 positionY = new Vector3(startPos.position.x + (i * percent), startPos.position.y, (startPos.position.z + y * percentY));

                Instantiate(carPrefab, positionY, Quaternion.identity);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(startPos.position + new Vector3(screenSize.x / 2, 0, screenSize.y / 2), new Vector3(screenSize.x, 1, screenSize.y));
    }
}

using UnityEngine;

public class ShakeObject : MonoBehaviour
{
    private float tension = 0;
    private Vector3 offsetPos;
    [SerializeField] private float maxOffset = 1;
    [SerializeField] private float maxTension = 3;
    [SerializeField] private float depletionRate = 8;
    
    public void AddTension(float value)
    {
        tension += value;
    }

    public Vector2 Shake()
    {
        if (tension > 0)
        {
            //set our shake value
            float shake = Mathf.Pow(tension, 2);

            //use noise to randomly shake our camera
            float offsetX = maxOffset * shake * Mathf.PerlinNoise(-1, 1) * Random.Range(-1, 1);
            float offsetY = maxOffset * shake * Mathf.PerlinNoise(-1, 1) * Random.Range(-1, 1);

            //make our target shake
            offsetPos = new Vector2(offsetX, offsetY);

        }

        //tension delpetes over time to start our shake intense and ease out
        tension -= depletionRate * Time.deltaTime;

        //you can add multiple instances of tension
        //for example if you hit a wall you add 1 tension, if you hit the wall 5 times in a row the shakes will get more intense
        //becase you add more tension

        //this script is meant to be dynamic based on how many objects are shaking the camera at once
        //and made sure to be capped out to not go too crazy
        tension = Mathf.Clamp(tension, 0, maxTension);
        Debug.Log(offsetPos + "||" + tension);
        return offsetPos;
    }
}

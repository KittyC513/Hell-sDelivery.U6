using System.ComponentModel.Design;
using UnityEngine;

public class SineBob : MonoBehaviour
{

    private Vector3 ApplySineWave(float frequency, float amplitude, Vector3 startPos, ref Vector3 changeInPos)
    {
        Vector3 currentPos = startPos + changeInPos;
        transform.localPosition = currentPos;
        float yPos = amplitude * Mathf.Sin(Time.time * frequency);
        changeInPos = new Vector3(0, yPos, 0);
        return currentPos;
    }
}

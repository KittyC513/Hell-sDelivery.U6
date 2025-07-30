using System.ComponentModel.Design;
using UnityEngine;

public class SineBob : MonoBehaviour
{
    [SerializeField] private float frequency = 1;
    [SerializeField] private float amplitude = 1;
    private Vector3 startPos;
    private Vector3 changeInPos;

    private void Start()
    {
        startPos = transform.position;
    }
    private void Update()
    {
        ApplySineWave(frequency, amplitude, startPos, ref changeInPos);
        transform.position = startPos + changeInPos;
    }
    private Vector3 ApplySineWave(float frequency, float amplitude, Vector3 startPos, ref Vector3 changeInPos)
    {
        Vector3 currentPos = startPos + changeInPos;
        transform.localPosition = currentPos;
        float yPos = amplitude * Mathf.Sin(Time.time * frequency);
        changeInPos = new Vector3(0, yPos, 0);
        return currentPos;
    }
}

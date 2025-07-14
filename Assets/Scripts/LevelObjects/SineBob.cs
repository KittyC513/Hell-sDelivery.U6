using System.ComponentModel.Design;
using UnityEngine;

public class SineBob : MonoBehaviour
{
    private Vector3 startPos;

    private Vector3 currentPos;
    private Vector3 changeInPos;

    [SerializeField] private float frequency = 1;
    [SerializeField] private float amplitude = 1;

    private void Start()
    {
        startPos = transform.localPosition;
    }
    private void Update()
    {
        currentPos = startPos + changeInPos;
        transform.localPosition = currentPos;
        float yPos = amplitude * Mathf.Sin(Time.time * frequency);
        changeInPos = new Vector3(0, yPos, 0);
    }
}

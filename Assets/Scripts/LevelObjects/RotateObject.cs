using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector3 rotateAxis;
    [SerializeField] private float rotateSpeed = 10;
    private float angle = 0;

    private void Update()
    {
        angle += rotateSpeed * Time.deltaTime;
        rectTransform.Rotate(rotateAxis.normalized, angle);
    }
}

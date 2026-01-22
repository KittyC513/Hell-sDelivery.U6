using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class HoldPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointsParent;
    private List<Transform> points;

    //all moving parts are under this object
    [SerializeField] private Transform platformParent;
    private List<HoldPlatController> platControllers;
    [SerializeField] private float platformMoveSpeed = 8;
    [SerializeField, Range(0, 1)] private float moveSpeedRandomRange = 0.25f;

    [SerializeField, Range(0, 1)] private float bobFrequencyRandomRange = 0.15f;
    [SerializeField] private float sineBobFrequency = 1;
    [SerializeField] private float sineBobHeight = 0.15f;
    private delegate void OnPlatformDeactivate();
    private delegate void OnPlatformActivate();
    private OnPlatformActivate onPlatformActivate;
    private OnPlatformDeactivate onPlatformDeactivate;
    

    private void Start()
    {
        points = new List<Transform>();
        platControllers = new List<HoldPlatController>();
        

        //add all the points to a list
        foreach (Transform child in pointsParent)
        {
            points.Add(child);
        }

        foreach (Transform child in platformParent)
        {
            HoldPlatController _plat = child.GetComponent<HoldPlatController>();
            
            if (_plat != null)
            {
                platControllers.Add(_plat);

                //initialize all the platforms
                _plat.InitializePlatform(this, platformMoveSpeed + Random.Range(-moveSpeedRandomRange, moveSpeedRandomRange), points, sineBobFrequency, sineBobHeight);

                //subscribe to the platform activate and deactivate functions
                onPlatformActivate += _plat.OnPlatformActivate;
                onPlatformDeactivate += _plat.OnPlatformDeactivate;
            }
            
        }

    }

    public void ActivatePlatform()
    {
        onPlatformActivate.Invoke();
    }

    public void DeactivatePlatform()
    {
        onPlatformDeactivate.Invoke();
    }

    
}

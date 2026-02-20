using System;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{   
    [SerializeField] public bool destroyOnAwake = false;
    [SerializeField] public bool destroyOnEnable = false;
    [SerializeField] private float delay = 2;

    private void Start()
    {
        if (destroyOnAwake)
        {
           Destroy(); 
        }
        
    }

    private void OnEnable()
    {
        if (destroyOnEnable)
        {
           Destroy(); 
        }
    }

    public void Destroy()
    {
        Destroy(this.gameObject, delay);
    }
    
}

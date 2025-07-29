using UnityEngine;
using UnityEngine.Events;

public class PlayerGate : MonoBehaviour
{
    [SerializeField] private PlayerSensor[] sensors;
    [SerializeField] public UnityEvent onActivate;
    [SerializeField] private bool active = false;

    private void Update()
    {
        bool allActive = true;

        for (int i = 0; i < sensors.Length; i++)
        {
            if (sensors[i].active == false)
            {
                allActive = false;
            }
        }

        if (allActive)
        {
            active = true;
          
        }

        if (active)
        {
            onActivate.Invoke();
        }
    }
}

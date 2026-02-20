using UnityEngine;
using UnityEngine.Events;

public class BombSwitch : MonoBehaviour
{

    [SerializeField] public UnityEvent onSwitchTrigger; //triggers once, when the trigger switch function is ran
    [SerializeField] public UnityEvent onSwitchExit; //triggers when the switch is no longer active
    [SerializeField] public UnityEvent whileSwitchActive; //triggers in update while the switch is active
    private bool triggered = false;

    [SerializeField] private Renderer render1;
    [SerializeField] private Renderer render2;

    [SerializeField] private bool timedSwitch = false;
    [SerializeField] private float switchTimer = 5;
    private float timeTemp;

    private void Start()
    {
        //render1?.material.SetInt("_Triggered", 0);
        //render2?.material.SetInt("_Triggered", 0);
    }

    private void Update()
    {
        if (timedSwitch)
        {
            TimedSwitch();
        }

        if (triggered)
        {
          
            whileSwitchActive.Invoke();
        }
    }

    public void TriggerSwitch()
    {
        if (!triggered)
        {
            onSwitchTrigger.Invoke();
        }

        //render1?.material.SetInt("_Triggered", 1);
        //render2?.material.SetInt("_Triggered", 1);
        timeTemp = 0;
        triggered = true;
    }

    public void TurnOffSwitch()
    {
        if (triggered)
        {
            onSwitchExit.Invoke();
        }
        //render1?.material.SetInt("_Triggered", 0);
        //render2?.material.SetInt("_Triggered", 0);
        triggered = false;
    }
    private void TimedSwitch()
    {
        if (triggered)
        {
            timeTemp += Time.deltaTime;

            if (timeTemp >= switchTimer)
            {
                TurnOffSwitch();
            }
        }
    }
}

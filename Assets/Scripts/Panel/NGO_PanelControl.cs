using System;
using UnityEngine;

public class NGO_PanelControl : MonoBehaviour
{
    public static NGO_PanelControl instance;
    public PlayerInputDetection inputDetector;
    public CustomGUIToggle toggleKeyboard;
    public CustomGUIToggle toggleGamepad;
    private bool isAddedEvent = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if(inputDetector != null && !isAddedEvent)
        {
            AddEvent();
        }
    }


    void AddEvent()
    {
        toggleKeyboard.toggleEvent += (value) =>
        {
            if (value)
            {
                inputDetector.inputDeviceType = E_InputDeviceType.keyboard;
            }
        };

        toggleGamepad.toggleEvent += (value) =>
        {
            if (value)
            {
                inputDetector.inputDeviceType = E_InputDeviceType.Gamepad;
            }
        };

        isAddedEvent = true;
    }
}

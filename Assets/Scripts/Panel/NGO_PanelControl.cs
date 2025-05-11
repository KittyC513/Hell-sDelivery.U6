using System;
using UnityEngine;

public class NGO_PanelControl : MonoBehaviour
{
    public static NGO_PanelControl instance;
    public PlayerInputDetection inputDetector;
    public CustomGUIToggle toggleKeyboard;
    public CustomGUIToggle toggleGamepad;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;

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
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum E_Slider_Type
{
    Horizontal,
    Vertical,
}
public class CustomGUISlider : CustomGUIControl
{
    public float minValue = 0;
    public float maxValue = 1;

    public float curValue = 0;

    public E_Slider_Type sliderType = E_Slider_Type.Horizontal;
    //小按钮的 style
    public GUIStyle styleThumb;

    public event UnityAction<float> sliderEvent;

    private float preValue;
    protected override void StyleOff()
    {
        switch (sliderType)
        {
            case E_Slider_Type.Horizontal:
                curValue = GUI.HorizontalSlider(guiPos.Pos, curValue, minValue, maxValue);
                break;
            case E_Slider_Type.Vertical:
                curValue = GUI.VerticalSlider(guiPos.Pos, curValue, minValue, maxValue);
                break;
        }

        if (preValue != curValue) 
        {
            sliderEvent?.Invoke(curValue);
            preValue = curValue;
        }

    }

    protected override void StyleOn()
    {
        switch (sliderType)
        {
            case E_Slider_Type.Horizontal:
                curValue = GUI.HorizontalSlider(guiPos.Pos, curValue, minValue, maxValue,style, styleThumb);
                break;
            case E_Slider_Type.Vertical:
                curValue = GUI.VerticalSlider(guiPos.Pos, curValue, minValue, maxValue, style, styleThumb);
                break;
        }

        if (preValue != curValue)
        {
            sliderEvent?.Invoke(curValue);
            preValue = curValue;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomGUIToggle : CustomGUIControl
{
    public bool isSelected;

    public event UnityAction<bool> toggleEvent;

    private bool isSameOption;
    protected override void StyleOff()
    {
        isSelected = GUI.Toggle(guiPos.Pos, isSelected, content);

        //只有变化时 才执行函数 
        if (isSameOption != isSelected)
        {
            toggleEvent?.Invoke(isSelected);
            isSameOption = isSelected;
        }

    }

    protected override void StyleOn()
    {
        isSelected = GUI.Toggle(guiPos.Pos, isSelected, content, style);

        if (isSameOption != isSelected)
        {
            toggleEvent?.Invoke(isSelected);
            isSameOption = isSelected;
        }

    }
}

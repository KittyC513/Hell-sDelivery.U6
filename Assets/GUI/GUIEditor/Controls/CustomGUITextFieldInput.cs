using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomGUITextFieldInput : CustomGUIControl
{

    public event UnityAction<string> textEvent;
    private string preString = "";
    protected override void StyleOff()
    {
        content.text = GUI.TextField(guiPos.Pos, content.text);
        if (preString != content.text)
        {
            textEvent?.Invoke(preString);
            preString = content.text;

        }
    }

    protected override void StyleOn()
    {
        content.text = GUI.TextField(guiPos.Pos, content.text, style);
        if (preString != content.text)
        {
            textEvent?.Invoke(preString);
            preString = content.text;
        }
    }
}

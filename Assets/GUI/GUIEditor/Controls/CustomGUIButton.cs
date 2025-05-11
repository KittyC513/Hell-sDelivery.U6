using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomGUIButton : CustomGUIControl
{
    //提供给外部 用于响应 按钮点击的事件 只要给外部给予响应函数 那就会执行
    public event UnityAction clickEvent;
    protected override void StyleOff()
    {
        if(GUI.Button(guiPos.Pos, content))
        {
            clickEvent?.Invoke();
        }
    }

    protected override void StyleOn()
    {
        if (GUI.Button(guiPos.Pos, content, style))
        {
            clickEvent?.Invoke();
        }
    }
}

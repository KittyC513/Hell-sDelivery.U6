using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Stype_Switch
{
    On,
    Off,
}
public abstract class CustomGUIControl : MonoBehaviour
{
    //提取控件的共同表现
    //位置信息
    public CustomGUIPos guiPos;
    //显示内容信息
    public GUIContent content;
    //自定义样式
    public GUIStyle style;
    //自定义样式是否开启
    public E_Stype_Switch styleSwitch = E_Stype_Switch.Off;

    //提供绘制方法
    public void DrawGUI()
    {
        switch (styleSwitch)
        {
            case E_Stype_Switch.On:
                StyleOn();
                break;
            case E_Stype_Switch.Off:
                StyleOff();
                break;
        }
    }

    /// <summary>
    /// 自定义样式开启时 的绘制方法
    /// </summary>
    protected abstract void StyleOn();


    /// <summary>
    /// 自定义样式关闭时 的绘制方法
    /// </summary>
    protected abstract void StyleOff();


}

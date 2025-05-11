using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


/// <summary>
/// 对其方式
/// </summary>
public enum E_Alignmenmt_Type 
{
    Top_left,
    Top_Center,
    Top_right,
    Center_left,
    Center,
    Center_right,
    Bottom_left,
    Bottom_center,
    Bottom_right,
}

/// <summary>
/// 该类 是用来表示位置 计算位置相关信息的 不需要继承mono
/// </summary>
[System.Serializable]
public class CustomGUIPos 
{
    //主要是处理 控件位置相关的内容
    //要完成 分辨率自适应的相关计算
    
    //该位置信息 会用来返回给外部 用于绘制控件
    //需要对它进行 计算
    private Rect oriPos = new Rect(0,0,100,100);

    //屏幕九宫格对齐方式
    public E_Alignmenmt_Type screen_Alignment_Type = E_Alignmenmt_Type.Center;
    //控件中心对齐方式
    public E_Alignmenmt_Type control_Center_Alignment_type =E_Alignmenmt_Type.Center;
    //偏移位置
    public Vector2 pos;
    //宽高
    public float width = 100;
    public float height = 50;

    //用于计算的 中心点 成员变量
    private Vector2 centerPos;

    //计算中心点偏移的方法
    private void CalcCenterPos()
    {
        switch (control_Center_Alignment_type)
        {
            case E_Alignmenmt_Type.Top_left:
                centerPos.x = 0;
                centerPos.y = 0;
                break;
            case E_Alignmenmt_Type.Top_Center:
                centerPos.x = -width / 2;
                centerPos.y = 0;
                break;
            case E_Alignmenmt_Type.Top_right:
                centerPos.x = -width;
                centerPos.y = 0;
                break;
            case E_Alignmenmt_Type.Center_left:
                centerPos.x = 0;
                centerPos.y = -height / 2;
                break;
            case E_Alignmenmt_Type.Center:
                centerPos.x = -width / 2;
                centerPos.y = -height / 2;
                break;
            case E_Alignmenmt_Type.Center_right:
                centerPos.x = -width;
                centerPos.y = -height / 2;
                break;
            case E_Alignmenmt_Type.Bottom_left:
                centerPos.x = 0;
                centerPos.y = -height;
                break;
            case E_Alignmenmt_Type.Bottom_center:
                centerPos.x = -width / 2;
                centerPos.y = -height ;
                break;
            case E_Alignmenmt_Type.Bottom_right:
                centerPos.x = -width;
                centerPos.y = -height;
                break;
        }
    }

    //计算最终相对坐标位置的方法
    private void CalcPos()
    {
        switch (screen_Alignment_Type)
        {
            case E_Alignmenmt_Type.Top_left:
                oriPos.x = centerPos.x + pos.x;
                oriPos.y = centerPos.y + pos.y;
                break;
            case E_Alignmenmt_Type.Top_Center:
                oriPos.x = Screen.width / 2 + centerPos.x + pos.x;
                oriPos.y = centerPos.y + pos.y;
                break;
            case E_Alignmenmt_Type.Top_right:
                oriPos.x = Screen.width + centerPos.x - pos.x;
                oriPos.y = centerPos.y + pos.y;
                break;
            case E_Alignmenmt_Type.Center_left:
                oriPos.x = centerPos.x + pos.x;
                oriPos.y = Screen.height / 2 + centerPos.y + pos.y;
                break;
            case E_Alignmenmt_Type.Center:
                oriPos.x = Screen.width / 2 + centerPos.x + pos.x;
                oriPos.y = Screen.height / 2 + centerPos.y + pos.y;
                break;
            case E_Alignmenmt_Type.Center_right:
                oriPos.x = Screen.width + centerPos.x - pos.x;
                oriPos.y = Screen.height / 2 + centerPos.y + pos.y;
                break;
            case E_Alignmenmt_Type.Bottom_left:
                oriPos.x = centerPos.x + pos.x;
                oriPos.y = Screen.height + centerPos.y - pos.y;
                break;
            case E_Alignmenmt_Type.Bottom_center:
                oriPos.x = Screen.width / 2 + centerPos.x + pos.x;
                oriPos.y = Screen.height + centerPos.y - pos.y;
                break;
            case E_Alignmenmt_Type.Bottom_right:
                oriPos.x = Screen.width + centerPos.x - pos.x;
                oriPos.y = Screen.height + centerPos.y - pos.y;
                break;
        }
    }

    /// <summary>
    /// 得到 最终绘制的位置 和 宽高
    /// </summary>
    public Rect Pos
    {
        get
        {
            //进行计算
            //计算中心点偏移
            CalcCenterPos();
            //计算 相对屏幕坐标点
            CalcPos();
            //宽高直接赋值 返回给外部 别人直接使用来绘制控件
            oriPos.width = width;
            oriPos.height = height;
            return oriPos;
        }
    }
}

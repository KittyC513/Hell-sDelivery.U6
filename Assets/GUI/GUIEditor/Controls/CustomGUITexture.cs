using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGUITexture : CustomGUIControl
{
    //图片绘制的缩放模式
    public ScaleMode scaleMode = ScaleMode.StretchToFill;

    protected override void StyleOff()
    {
        if(content.image != null)
        {
            GUI.DrawTexture(guiPos.Pos, content.image, scaleMode);
        }

    }

    protected override void StyleOn()
    {
        if (content.image != null)
        {
            GUI.DrawTexture(guiPos.Pos, content.image, scaleMode);
        }
    }
}

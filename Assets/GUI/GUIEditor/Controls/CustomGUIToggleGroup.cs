using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGUIToggleGroup : MonoBehaviour
{
    public CustomGUIToggle[] toggles;

    private CustomGUIToggle preToggle;
    // Start is called before the first frame update
    void Start()
    {
        if (toggles.Length == 0)
            return;

        //通过遍历 来为多个 多选框 添加 监听事件函数
        //在函数中做 处理
        //当一个为true时 其它变成 false
        for (int i = 0; i < toggles.Length; i++) 
        {
            CustomGUIToggle toggle = toggles[i];
            toggle.toggleEvent += (value) =>
            {
                //当传入的 value 是 true时 其它要变成 false
                if (value)
                {
                    for (int j = 0; j < toggles.Length; j++)
                    {
                        //这里是闭包 toggle就是上一个函数中申明变量
                        //改变了它的生命周期
                        if (toggles[j] != toggle)
                        {
                            toggles[j].isSelected = false;
                        }
                    }
                    preToggle = toggle;
                }
                //判断 当前变成false的toggle 是不是上一次为true
                //如果是 就不应该让它变成false
                else if(toggle == preToggle)
                {
                    toggle.isSelected = true;
                }
            };
        } 
    }

}

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ManualBillboardUI : BillboardUI
{
    [SerializeField] private GameObject canvasp1;
    [SerializeField] private GameObject canvasp2;

    protected override void SetupBillboard()
    {
        p1Cam = GameManager.instance.cam_p1;
        p2Cam = GameManager.instance.cam_p2;
        images = new List<GameObject>();

        images.Add(canvasp1);
        images.Add(canvasp2);


        if (!globalIcons)
        {
            images[0].layer = LayerMask.NameToLayer("UI_P2Ignore");
            images[1].layer = LayerMask.NameToLayer("UI_P1Ignore");
        }
        else
        {
            images[0].GetComponentInChildren<Image>().color = Color.red;
            images[1].GetComponentInChildren<Image>().color = Color.blue;
        }

        onInitialize?.Invoke();
    }
}

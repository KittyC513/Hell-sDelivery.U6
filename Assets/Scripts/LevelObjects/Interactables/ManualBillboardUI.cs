using UnityEngine;
using System.Collections.Generic;

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

        onInitialize?.Invoke();
    }
}

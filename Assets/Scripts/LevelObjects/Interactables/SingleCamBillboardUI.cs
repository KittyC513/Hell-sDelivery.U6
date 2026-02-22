using UnityEngine;

public class SingleCamBillboardUI : BillboardUI
{
    [SerializeField] public Camera cameraToBillboard;

    protected override void SetupBillboard()
    {
        //create our single UI element
        images.Add(Instantiate(billboardPrefab, this.transform.position, Quaternion.identity, this.transform).gameObject); 
        onInitialize?.Invoke();
    }

    protected override void ToggleUI()
    {
        //make sure everything needed to function is not null
        if (cameraToBillboard == null || !active || images[0] == null) return;

        //update the unaltered position
        startPos = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);

        //if either player toggle is active show it to the single camera
        if (player1Active || player2Active)
        {
            images[0].SetActive(true);
            BillboardToCamera(cameraToBillboard, images[0]);
        }
        else
        {
            images[0].SetActive(false);
        }
        
    }
}

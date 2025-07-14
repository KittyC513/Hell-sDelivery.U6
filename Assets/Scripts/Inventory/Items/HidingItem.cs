using UnityEngine;

public class HidingItem : ItemBase
{
    public GameObject itemHolder;
    public string layerMask = "Invisible_";
    public bool isChanged_layer = false;

    private Bag bag;

    private string playerLayerName;
    public override void UseFunction()
    {
        itemHolder = playerLockOn.gameObject;
        if (!isChanged_layer)
        {
            playerLayerName = LayerMask.LayerToName(itemHolder.layer);
            itemHolder.layer = LayerMask.NameToLayer(layerMask + playerLayerName);
            bag = itemHolder.GetComponent<Bag>();
            bag.isInvisible = true;
            isChanged_layer = true;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (itemHolder != null)
        {
            if (isOnUse && !inputDetection.crouchPressed || !isOnUse)
            {
                if (isChanged_layer)
                {
                    itemHolder.layer = LayerMask.NameToLayer(playerLayerName);
                    isChanged_layer = false;
                    bag.isInvisible = false;
                }
            }
        }
        else
        {
            bag = null;
            playerLayerName = "";
        }

    }
}

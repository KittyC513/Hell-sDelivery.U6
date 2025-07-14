using UnityEngine;

public class HidingItem : ItemBase
{
    public GameObject itemHolder;
    public string layerMask = "Invisible_";
    public bool isChanged_layer = false;

    private Bag bag;

    private string playerLayerName;

    public Animator animator;
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

            bag.playerMod.SetActive(false);
        }
        animator.SetBool("isHiding",true);
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
                animator.SetBool("isHiding", false);
                bag.playerMod.SetActive(true);
            }


        }
        else
        {
            bag = null;
            playerLayerName = "";

            animator.SetBool("isHiding", false);            
            if(bag  != null)
                bag.playerMod.SetActive(false);
        }

    }
}

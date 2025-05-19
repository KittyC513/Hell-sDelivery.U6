using UnityEngine;

public class BombItem : MonoBehaviour
{
    private ItemHandler iHandler;
    private PlayerLockOn playerLockOn;

    private void Awake()
    {
        iHandler = GetComponent<ItemHandler>();
        if (iHandler == null) Debug.Log(this.ToString() + " needs an itemHandler attached on the gameObject " + this.gameObject.name);
    }

    private void Start()
    {
        iHandler.onItemTrigger += ThrowTowardTarget;
    }

    private void OnDisable()
    {
        iHandler.onItemTrigger -= ThrowTowardTarget;
    }

    private void ThrowTowardTarget()
    {
        print("Throw");
    }

}

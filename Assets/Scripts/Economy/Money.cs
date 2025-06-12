using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] private Collectable collectable;
    [SerializeField] private int amount;

    private void OnEnable()
    {
        if (collectable != null)
        {
            collectable.onCollect += AddValueToPlayer;
        }
    }

    private void OnDisable()
    {
        if (collectable != null)
        {
            collectable.onCollect -= AddValueToPlayer;
        }
    }

    private void AddValueToPlayer(GameObject collector)
    { 
        //if the collector is a player
        if (collector.CompareTag("Player"))
        {
            //grab the player's money manager
            PlayerMoneyManager moneyManager = collector.GetComponent<PlayerMoneyManager>();

            //double check if its null
            if (moneyManager != null )
            {
                moneyManager.AddMoney(amount);
            }
        }
    }
}

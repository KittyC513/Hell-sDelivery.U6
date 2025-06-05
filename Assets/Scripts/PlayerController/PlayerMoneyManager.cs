using UnityEngine;

public class PlayerMoneyManager : MonoBehaviour
{
    //this value needs to be set by the player manager so that each player has their respective stats object
    [SerializeField] public PlayerStats playerStats;

    [SerializeField] private LayerMask moneyMask;
    [SerializeField] private float magnetRange = 2; //how far away money can be detected to the player
    [SerializeField] private float magnetSpeed = 5; // how fast the maximum move speed of the money is


    private int currentMoney = 0;

    public delegate void OnMoneyChange();
    public OnMoneyChange onMoneyChange;

    private Collider[] detectedMoney;

    private void Start()
    {
        //this is how many loose dollars can be on the ground at once without missing some of them (i think 100 is pretty good and we prob wont want more than that anyways)
        detectedMoney = new Collider[100];
    }
    private void OnEnable()
    {
        //subscrive to the on money change event
        onMoneyChange += UpdateMoneyCount;
    }

    private void OnDisable()
    {
        onMoneyChange -= UpdateMoneyCount;
    }

    private void FixedUpdate()
    {
        MagnetMoneyToPlayer();
    }

    public void AddMoney(int value)
    {
        //add money to the player
        currentMoney += value;

        //trigger an on money change event for any listeners
        onMoneyChange?.Invoke();
    }

    private void UpdateMoneyCount()
    {
        //update the scriptable object
        playerStats.currentMoneyCount = currentMoney;
    }

    private void MagnetMoneyToPlayer()
    {
        int temp = Physics.OverlapSphereNonAlloc(transform.position, magnetRange, detectedMoney, moneyMask);

        if (temp > 0)
        {
            for (int i = 0; i < detectedMoney.Length; i++)
            {
                if (detectedMoney[i] != null)
                {
                    Rigidbody mRb;

                    mRb = detectedMoney[i].GetComponentInParent<Rigidbody>();

                    if (mRb != null)
                    {
                        //get a reference to the money we are collecting
                        LooseMoney looseMoney = mRb.GetComponentInParent<LooseMoney>();

                        if (looseMoney.activated)
                        {
                            float distanceFromPlayer = Vector3.Distance(transform.position, mRb.position);
                            Vector3 directionToPlayer = (transform.position - mRb.position).normalized;

                            mRb.AddForce((magnetSpeed / (distanceFromPlayer)) * directionToPlayer, ForceMode.Impulse);
                        }
                    }
                }
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("LooseMoney"))
        {
            //get a reference to the money we are collecting
            LooseMoney looseMoney = other.GetComponentInParent<LooseMoney>();

            if (looseMoney.activated)
            {
                //add the value and tell the money that is has been collected
                AddMoney(looseMoney.value);
                looseMoney.Collect();
            }
        }
    }

}

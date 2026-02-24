
using UnityEngine;
using UnityEngine.UI;

public class FriendshipBerry : MonoBehaviour
{
    [SerializeField] public enum BerryState { floating, equipped, collected }
    private BerryState berryState;

    [SerializeField] private FollowerObject followObject;
    private Collider col;
    private FollowerQueue queue;
    private FollowerQueue otherQueue; //the queue of the player not holding this item
    [SerializeField] private float captureTime = 5;
    [SerializeField] private float returnSpeed = 2;
    [SerializeField] private float minRequiredDistance = 3;
    [SerializeField] private ParticleSystem collectParticles;
    [SerializeField] private BillboardUI billboardUI;
    [SerializeField] private Image p1Image;
    [SerializeField] private Image p2Image;
    private float t;
    private float lerpTime;
    private Vector3 startPos;
    private Vector3 newPos;


    private GameObject otherPlayer;
    private GameObject currentPlayer;
    private Animator anim;

    private void Start()
    {
        startPos = transform.position;
        col = GetComponent<Collider>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        switch (berryState)
        {
            case BerryState.floating:
                WhileFloating();
            break;
            case BerryState.equipped:
                WhileCaptured();
            break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCollider"))
        {
            //grab a follower queue
            queue = other.GetComponent<FollowerQueue>();

            if (queue != null)
            {
                //collect the berry - add to queue, activate follower object
                followObject.ToggleActive(true);
                queue.AddNewFollower(followObject);
                ChangeState(BerryState.equipped);

                //grab the current player 
                currentPlayer = other.transform.parent.gameObject;
                int playerNum = currentPlayer.GetComponent<PlayerInputDetection>().playerNum;

                //grab the other player
                if (playerNum == 1)
                {
                    otherPlayer = GameManager.instance.player2;
                    otherQueue = otherPlayer.GetComponentInChildren<FollowerQueue>();
                    billboardUI.ShowIconToPlayer(true, 1);
                    billboardUI.ShowIconToPlayer(false, 2);
                }
                else
                {
                    otherPlayer = GameManager.instance.player1;
                    otherQueue = otherPlayer.GetComponentInChildren<FollowerQueue>();
                    billboardUI.ShowIconToPlayer(true, 2);
                    billboardUI.ShowIconToPlayer(false, 1);
                }
                
            }
        }
    }

    private void ChangeState(BerryState state)
    {
        //basically on state enter functions
        switch(state)
        {
            case BerryState.floating:

                if (berryState == BerryState.equipped)
                {
                    OnCaptureExpire();
                }
                
                break;
            case BerryState.equipped:

                if (berryState == BerryState.floating)
                {
                    OnCapture();
                }

                break;
            case BerryState.collected:

                OnCollect();

                break;
        }

        //update the new state
        berryState = state;
    }
    
    public void WhileFloating()
    {
        if (lerpTime < 1)
        {
            //call to move position back if not reset already
            ResetPosition(newPos, startPos);
        }
        else
        {
            if (!col.enabled) col.enabled = true;
            if (transform.position != startPos) transform.position = startPos;
            Debug.Log(transform.position + "||" + startPos);
        }
        
    }
    public void WhileCaptured()
    {
        Timer();
        CompareDistance();
        p1Image.fillAmount = (captureTime - t) / captureTime;
        p2Image.fillAmount = (captureTime - t) / captureTime;
       
    }

    public void OnCaptureExpire()
    {
        //give the lerp our current position
        newPos = transform.position;

        //reset the lerp time
        lerpTime = 0;
        followObject.ToggleActive(false);
        queue.RemoveFollower(followObject);
        queue = null;


        billboardUI.DisableAllIcons();
        
    }
    public void OnCapture()
    {
        //reset timer
        t = 0;
        
        col.enabled = false;
    }

    private void ResetPosition(Vector3 currentPos, Vector3 resetPos)
    {
        lerpTime += returnSpeed * Time.deltaTime;
        float percent = lerpTime / 1;

        transform.position = Vector3.Lerp(currentPos, resetPos, percent);
    }

    public void Timer()
    {
        t += Time.deltaTime;

        if (t >= captureTime)
        {
            ChangeState(BerryState.floating);
        }
    }

    private void OnCollect()
    {
        queue.RemoveFollower(followObject);
        anim.SetTrigger("Collected");
        billboardUI.DisableAllIcons();
        //temporary
        //Destroy(this.gameObject);
    }

    public void Destroy()
    {
        Destroy(this.gameObject, 0.15f);
    }

    private void CompareDistance()
    {
        float distance = Vector3.Distance(currentPlayer.transform.position, otherPlayer.transform.position);
        
        if (distance < minRequiredDistance && otherQueue.DoesQueueContainTag("Berry") != null)
        {
            otherQueue.DoesQueueContainTag("Berry").GetComponent<FriendshipBerry>().CollectBerry();
            CollectBerry();
        }
    }

    public void PlayParticleSystem()
    {
        collectParticles.Play();
    }

    public void CollectBerry()
    {
        if (berryState != BerryState.collected) ChangeState(BerryState.collected);
    }

}

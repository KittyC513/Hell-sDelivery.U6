using UnityEngine;

public class FriendshipBerry : MonoBehaviour
{
    [SerializeField] public enum BerryState { floating, equipped, collected }
    private BerryState berryState;

    [SerializeField] private FollowerObject followObject;
    private Collider col;
    private FollowerQueue queue;
    [SerializeField] private float captureTime = 5;
    [SerializeField] private float returnSpeed = 2;
    [SerializeField] private float minRequiredDistance = 3;
    private float t;
    private float lerpTime;
    private Vector3 startPos;
    private Vector3 newPos;


    private GameObject otherPlayer;
    private GameObject currentPlayer;

    private void Start()
    {
        startPos = transform.position;
        col = GetComponent<Collider>();
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
                }
                else
                {
                    otherPlayer = GameManager.instance.player1;
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
        }
        
    }
    public void WhileCaptured()
    {
        Timer();
        CompareDistance();
    }

    public void OnCaptureExpire()
    {
        //give the lerp our current position
        newPos = transform.position;

        //reset the lerp time
        lerpTime = 0;
        
        queue.RemoveFollower(followObject);
        queue = null;
 
        
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
        //temporary
        Destroy(this.gameObject);
    }

    private void CompareDistance()
    {
        float distance = Vector3.Distance(currentPlayer.transform.position, otherPlayer.transform.position);

        if (distance < minRequiredDistance)
        {
            ChangeState(BerryState.collected);
        }
    }

}

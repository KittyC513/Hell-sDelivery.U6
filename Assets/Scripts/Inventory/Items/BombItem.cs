
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BombItem : ItemBase
{
    //public ItemHandler iHandler;

    public Transform targetPos;
    public float dropSpeed = 5f;

    public GameObject bombPrefab;

    [Header("Bomb")]
    public Vector3 offset; //to replace bombs
    public int maxBombs = 2;
    public int numOfBombs = 0;
    public GameObject bombModel;

    //cooldown setting
    public float cdSpawn = 0.3f;
    public float timer = 0;

    public List<BombMovement> bombsList = new List<BombMovement>();

    public PlayerStateMachine pStateMachine;
    public bool validatedUse = false;

    [Header("Bomb Throwing")]
    public float maxChargeTime;
    private float chargeHoldTime;
    public float maximumThrowForce;
    public float minimumThrowForce;
    private bool bombPulled = false;
    private bool charging = false;

    private Vector3 bombHoldPoint;
    private GameObject bomb;
    [SerializeField] private float yThrowOffset = 0;
    [SerializeField] private float forwardThrowOffset = 0;


    public override void Initialize()
    {
        base.Initialize();
        onItemDropped += OnRemoved;
    }

    public override void UseFunction()
    {
        if (validatedUse)
        {
            //Heres the way its gonna work
            //on use pull out a bomb and hold it above your head
                //in the future this should override animation
                //bomb pulled is now true (player cannot pull another bomb)
            //on another press down start throwing the bomb
            //on release throw the bomb with maximum force * buttonHoldTime / maxChargeTime

            //use the throw arc to show the bomb trajectory

            if (!bombPulled)
            {
                PullBomb();
                return;
            }
            else //bomb is already pulled
            {
                // start charging
                charging = true;
            }

            //ThrowBomb();
            //print("Bomb is thrown");
        }
    }

    private void OnRemoved()
    {
        if (bomb != null)
        {
            Destroy(bomb);
            
        }

        bombPulled = false;
        charging = false;
        chargeHoldTime = 0;
    }

    
    private void PullBomb()
    {
        
        bombHoldPoint = new Vector3(currentBag.transform.position.x, currentBag.transform.position.y + 2, currentBag.transform.position.z);
        bomb = Instantiate(bombPrefab, bombHoldPoint, Quaternion.identity, SceneManager.GetActiveScene().GetRootGameObjects()[0].transform);

        bomb.transform.position = bombHoldPoint;
        bombPulled = true;
             
    }

    private void HoldBomb()
    {
        bombHoldPoint = new Vector3(currentBag.transform.position.x, currentBag.transform.position.y + 2, currentBag.transform.position.z);

        if(bomb != null)
            bomb.transform.position = bombHoldPoint;
    }

    private void ChargeBombThrow()
    {
        //player is holding down the button, charge the bomb throw
        if (inputDetection.crouchPressed)
        {
            throwArc = currentBag.gameObject.GetComponent<ThrowArc>();

            if (chargeHoldTime < maxChargeTime)
            {
                chargeHoldTime += Time.deltaTime;
            }
            //clamp the charge hold time value
            chargeHoldTime = Mathf.Clamp(chargeHoldTime, 0, maxChargeTime);

            //get a percentage of charge completion
            float percent = chargeHoldTime / maxChargeTime;
            Camera cam = GameManager.Instance.cam_p1;
            if (inputDetection.playerNum == 2) cam = GameManager.Instance.cam_p2;
            //get the forward direction (y value is temporary)
            Vector3 dir = new Vector3(currentBag.transform.forward.x + forwardThrowOffset, cam.transform.forward.y + yThrowOffset, currentBag.transform.forward.z + forwardThrowOffset).normalized;
            
            //velocity is direction * force
            Vector3 velocity = dir * (maximumThrowForce * percent);

            throwArc.ShowThrowArc(velocity, bomb.transform.position, percent, gravity);
        }
        else
        {
            throwArc = currentBag.gameObject.GetComponent<ThrowArc>();
             //get a percentage of charge completion
            float percent = chargeHoldTime / maxChargeTime;
            
            Camera cam = GameManager.Instance.cam_p1;
            if (inputDetection.playerNum == 2) cam = GameManager.Instance.cam_p2;

            //get the forward direction (y value is temporary)
            Vector3 dir = new Vector3(currentBag.transform.forward.x + forwardThrowOffset, cam.transform.forward.y  + yThrowOffset, currentBag.transform.forward.z + forwardThrowOffset).normalized;
            
            //velocity is direction * force
            Vector3 velocity = dir * (maximumThrowForce * percent);

            throwArc.StopThrowArc();
            bomb.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            bomb.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.Impulse);

            if(bombsList.Count > maxBombs - 1)
            {
                for (int i = 0; i < bombsList.Count; i++)
                {
                    //remove any null objects from the bomb list
                    if (bombsList[i] == null)
                    {
                        bombsList.RemoveAt(i);
                    }
                }

                //check again if the bombs are above the max after removing any null bombs
                if(bombsList.Count > maxBombs - 1)
                {
                     //defuse old bomb
                    Transform bombPos = bombsList[0].transform;
                    bombsList.RemoveAt(0);

                    GameObject visualEffectObj = Instantiate(Resources.Load<GameObject>("Prefabs/VisualEffects/Bomb_defusing"), bombPos.position, Quaternion.identity);
                    List<BombMovement> bombsList2 = new List<BombMovement>();

                    Destroy(visualEffectObj, 1f);
                    Destroy(bombPos.gameObject, 1f);
                    /********************************************************************************************/
                    //audio source


                    /********************************************************************************************/
                    foreach (BombMovement item in bombsList) 
                    { 
                        bombsList2.Add(item);            
                    }
                    bombsList = bombsList2;
                    //add new bomb into the list
                    bombsList.Add(bomb.GetComponent<BombMovement>());
                }
                else
                {
                    bombsList.Add(bomb.GetComponent<BombMovement>());
                }
               
            }
            else
            {
                bombsList.Add(bomb.GetComponent<BombMovement>());
            }


            chargeHoldTime = 0;
            bomb = null;
            charging = false;
            bombPulled = false;
            //apply the force to the bomb we pulled
        }
        
    }

    private void ThrowBomb()
    {
        if (numOfBombs < maxBombs)
        {
            if (timer >= cdSpawn)
            {
                print(cdSpawn + " seconds cooldown is over, you can throw a bomb now");
                // while throwing bomb, generating a bomb object and reset the position to player's pos

                GameObject bombObj = Instantiate(bombPrefab);
                bombObj.transform.position = this.transform.position;
                //bombObj.transform.rotation = iHandler.iControl.transform.rotation;
                //if (Vector3.Distance(bombObj.transform.position, this.transform.position) < 0.1f)
                BombMovement bombMovement = bombObj.GetComponent<BombMovement>();
                bombMovement.playerLockOn = this.transform.parent.GetComponent<PlayerLockOn>();

                if (bombMovement.playerLockOn.lockTarget != null)
                    bombMovement.targetPos = bombMovement.playerLockOn.lockTarget.transform;

                bombsList.Add(bombMovement);
                numOfBombs++;
                timer = 0;
            }

        }

    }
    public void FixedUpdate()
    {
        //cooldown timer starts
        if (timer < cdSpawn)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = cdSpawn;
        }
     
    }

    protected override void Update()
    {
        base.Update();
        ValidatedCheck();

        if (bombPulled) HoldBomb();
        if (charging && bombPulled && bomb != null) ChargeBombThrow();
        if (bombPulled && bomb == null)
        {
            charging = false;
            bombPulled = false;
        }

    }
    private void ValidatedCheck()
    {
        if (isOnUse)
        {
            if(pStateMachine != null)
            {
                if (pStateMachine.showCurrentState == PlayerStateMachine.PlayerStates.freeFall)
                    validatedUse = false;
                else
                    validatedUse = true;
            }
            else
            {
                pStateMachine = this.transform.parent.GetComponent<PlayerStateMachine>();
            }
        }
        else
        {
            validatedUse = false;
            pStateMachine = null;
        }
    }

}

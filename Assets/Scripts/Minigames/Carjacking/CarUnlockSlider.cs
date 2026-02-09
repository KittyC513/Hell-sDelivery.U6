using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class CarUnlockSlider : MonoBehaviour
{
    [SerializeField] public RectTransform slider;
    [SerializeField] public RectTransform handle;
    [SerializeField] public RectTransform hitArea;

    private float sliderProgression = 0;
    [SerializeField] private float sliderSpeed = 1;
    private float startSpeed;

    [SerializeField] private int cyclesToComplete = 3;
    private int cyclesFinished = 0;

    private float startValue;
    private float endValue;
    private int dir;

    public bool completed = false;
    public bool active = false;
    private GameObject targetObj;
    [SerializeField] private float speedIncrease = 3;
    [SerializeField] private Camera mainCam;
    [SerializeField] private Image lockImg;
    private bool canInput = false;
    private PlayerInputDetection currentPlayer;
    private PlayerInteractor playerInteractor;
    private Health playerHealth;

    //attemping to fit the slider into world space at any rotation
    private Vector3 startPos;
    private Vector3 endPos;
    [SerializeField] private bool isMinigameCar = false;

    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform endTransform;
    [SerializeField] private Vector2 targetBarRange = new Vector2(0.55f, 1.5f);

    [SerializeField] private SoundEffectPlayer sfxPlayer;

    [SerializeField] private ShakeObject shakeObject;

    private Vector3 activatePos;

    private void Start()
    {
        slider.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        startSpeed = sliderSpeed;
        //PickNewSection(Random.Range(0.55f, 1.5f));
    }

    public void ResetMinigame()
    {
        //reset variables
        cyclesFinished = 0;
        sliderSpeed = startSpeed;
        PickNewSection(Random.Range(targetBarRange.x, targetBarRange.y));
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.onTakeDamage -= CancelMinigame;
            playerHealth = null;
        }
        
    }

    public void Activate(GameObject target, Vector3 playerPos, PlayerInputDetection playerInput)
    {
        //setup variables
        
        if (playerHealth == null)
        {
            playerHealth = playerInput.GetComponentInChildren<Health>();
            playerHealth.onTakeDamage += CancelMinigame;
        }

        //make the interacting player unable to interact with other objects while this game is active
        playerInteractor = playerInput.GetComponent<PlayerInteractor>();
        playerInteractor.ToggleCanInteract(false, this.name);

        //the car gameobject
        targetObj = target;

       

        //the position to place this object (car unlock slider)
        Vector3 position = playerPos;
        transform.position = new Vector3(position.x, position.y + 1, position.z);
        slider.gameObject.SetActive(true);

        activatePos = transform.position;
        //get a reference to the inputting player
        currentPlayer = playerInput;

        active = true;

        //find the start and end of the slider 
        startValue = slider.position.x - (slider.rect.width / 2);
        endValue = slider.position.x + (slider.rect.width / 2);

        //set a new section
        PickNewSection(Random.Range(targetBarRange.x, targetBarRange.y));
    }

    public void PlayUnlockUI(Vector3 worldPos)
    {
        Vector3 worldToScreen;
        //plays a simple UI indicating a car was unlocked
        if (isMinigameCar)
        {
            worldToScreen = mainCam.WorldToScreenPoint(worldPos);
        }
        else
        {
             worldToScreen = currentPlayer.cam.WorldToScreenPoint(worldPos);
        }
        
        lockImg.gameObject.SetActive(true);
        lockImg.rectTransform.position = worldToScreen;
        lockImg.GetComponent<Animator>().Play("LockBreak", -1, 0f);
        
    }

    public void CancelMinigame(Vector3 dir)
    {
        EndMinigame(false);
    }

    public void EndMinigame(bool win)
    {
        //unfreeze the player who just ended their minigame
        if (currentPlayer.playerNum == 1)
        {
            GameManager.instance.UnFreezePlayer1();
        }
        else
        {
            GameManager.instance.UnFreezePlayer2();
        }

        if (win)
        {
            Debug.Log("Unlocked");
            //unlock the car
            PlayUnlockUI(targetObj.transform.position);
            targetObj.GetComponent<CarjackCar>().UnlockCar(currentPlayer);
        }
        else
        {
            //fail the minigame
            Debug.Log("Failed");
        }

        //Allow the player to interact with objects again
        playerInteractor.ToggleCanInteract(true, this.name);

        //set back to inactive
        active = false;
        slider.gameObject.SetActive(false);
        targetObj = null;
    }

    private void Update()
    {
        if (slider.gameObject.activeSelf)
        {
            Vector2 shakePos = shakeObject.Shake();
            transform.position = new Vector3(activatePos.x + shakePos.x, activatePos.y, activatePos.z + shakePos.y);
            //startPos = slider.position.x - (slider.rect.width / 2);
            //endValue = slider.position.x + (slider.rect.width / 2);
            
            //width * direction to the start of the slider
            Vector3 dirToStart = (startTransform.position - slider.position).normalized;
            Vector3 dirToEnd = (endTransform.position - slider.position).normalized;
            
            startPos = slider.position + (slider.rect.width / 2 * dirToStart);
            endPos = slider.position + (slider.rect.width / 2 * dirToEnd);

            //move the handle position from the start to the end
            //handle.position = new Vector3(Mathf.Lerp(startValue, endValue, sliderProgression / 10), handle.position.y, handle.position.z);

        
            handle.position = Vector3.Lerp(startPos, endPos, sliderProgression / 10);
            //progress the slider
            sliderProgression += (sliderSpeed * dir) * Time.deltaTime;

            //reverse the slider if it goes to the end and start
            if (sliderProgression >= 10)
            {
                sliderProgression = 10;
                dir = -1;
            }
            else if (sliderProgression <= 0)
            {
                sliderProgression = 0;
                dir = 1;
            }

            //read inputs for the active slider
            if (currentPlayer?.interactPressed == true && canInput)
            {
                canInput = false;
                CallInput();
            }


            //make sure they cant hold down the button
            if (currentPlayer?.interactPressed!= true)
            {
                if (canInput == false)
                {
                    canInput = true;
                }
            }

            if (targetObj != null)
            {
                //check if the car got unlocked, if so boot the player out
                if (targetObj.GetComponent<CarjackCar>().unlocked)
                {
                    EndMinigame(false);
                }
            }

            if (!isMinigameCar && currentPlayer.playerCam != null && active)
            {
                this.transform.LookAt(currentPlayer.cam.transform.position);
            }
            
        }

       
       
    }

    //this is called from outside this script when the player attempts to input
    //this will either fail the minigame if the conditions are not met or complete the cycle
    public void CallInput()
    {
        //if the slider is on the target and the player inputs then they successfully cleared one cycle
        if (CheckSliderToTarget())
        {
            if (cyclesFinished + 1 >= cyclesToComplete)
            {
                
                sfxPlayer.PlaySoundEffect("ObjectBank1", "CarUnlock");
                
                EndMinigame(true);
            }
            else
            {
                shakeObject.AddTension(1.5f);
                sfxPlayer.PlaySoundEffect("ObjectBank1", "LockPick");
                cyclesFinished += 1;
                sliderSpeed += speedIncrease;
                PickNewSection(Random.Range(targetBarRange.x, targetBarRange.y));
            }            
            
        }
        else //the player missed
        {
            sfxPlayer.PlaySoundEffect("ObjectBank1", "LockFail");
            sfxPlayer.PlaySoundEffect("ObjectBank1", "FailJingle_1");
            EndMinigame(false);
        }
    }

    private void PickNewSection(float xSize)
    {
        //pick a section on the slider between start and end value but with a buffer of half the Xsize
        //pick a random percentage
        hitArea.sizeDelta = new Vector2(xSize, hitArea.rect.height);

        float randomPercent = Random.Range(0, 1f);

        //width * direction to the start of the slider
        Vector3 dirToStart = (startTransform.position - slider.position).normalized;
        Vector3 dirToEnd = (endTransform.position - slider.position).normalized;
        
        Vector3 _startPos = slider.position + (slider.rect.width / 2 * dirToStart);
        Vector3 _endPos = slider.position + (slider.rect.width / 2 * dirToEnd);
  
        Vector3 startPos = Vector3.Lerp(_startPos, _endPos, randomPercent);

        //check if the target area will go over the slider values
        if (startPos.x + (xSize / 2) > endValue)
        {
            startPos.x -= (startPos.x + (xSize / 2)) - endValue;
        }
        else if (startPos.x - (xSize / 2) < startValue)
        {
            startPos.x += startValue - (startPos.x - (xSize / 2));
        }

        hitArea.position = startPos;

    }
    
    private bool CheckSliderToTarget()
    {
        bool onTarget = false;

        //get the leftmost position of the current target section
        float leftPos = hitArea.localPosition.x - (hitArea.sizeDelta.x / 2);

        //get the rightmost pos
        float rightPos = hitArea.localPosition.x + (hitArea.sizeDelta.x / 2);

        //compare the ends of the handle to the ends of the target position to check if the handle is within the bounds of the target area
        if (handle.localPosition.x + handle.sizeDelta.x > leftPos && handle.localPosition.x - handle.sizeDelta.x < rightPos)
        {
            onTarget = true;
        }

       

        return onTarget;
    }
}

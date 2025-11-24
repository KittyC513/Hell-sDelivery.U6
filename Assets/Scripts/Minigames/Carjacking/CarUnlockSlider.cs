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

    private void Start()
    {
        slider.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        startSpeed = sliderSpeed;
        
    }

    public void ResetMinigame()
    {
        //reset variables
        cyclesFinished = 0;
        sliderSpeed = startSpeed;
        PickNewSection(Random.Range(0.75f, 2.5f));
    }

    public void Activate(GameObject target)
    {
        targetObj = target;
        Vector3 position = target.transform.position;
        transform.position = new Vector3(position.x, position.y + 1, position.z);
        slider.gameObject.SetActive(true);
        active = true;
        //find the start and end of the slider 
        startValue = slider.position.x - (slider.rect.width / 2);
        endValue = slider.position.x + (slider.rect.width / 2);
    }

    public void EndMinigame(bool win)
    {
        
        if (win)
        {
            Debug.Log("Unlocked");
            //unlock the car
            targetObj.GetComponent<CarjackCar>().UnlockCar();
        }
        else
        {
            //fail the minigame
            Debug.Log("Failed");
        }

        //set back to inactive
        active = false;
        slider.gameObject.SetActive(false);
        targetObj = null;
    }

    private void Update()
    {
        if (slider.gameObject.activeSelf)
        {
            //move the handle position from the start to the end
            handle.position = new Vector3(Mathf.Lerp(startValue, endValue, sliderProgression / 10), handle.position.y, handle.position.z);

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
                EndMinigame(true);
            }
            else
            {
                cyclesFinished += 1;
                sliderSpeed += speedIncrease;
                PickNewSection(Random.Range(0.75f, 2.5f));
            }            
            
        }
        else //the player missed
        {
            EndMinigame(false);
        }
    }

    private void PickNewSection(float xSize)
    {
        //pick a section on the slider between start and end value but with a buffer of half the Xsize
        //pick a random percentage
        hitArea.sizeDelta = new Vector2(xSize, hitArea.rect.height);

        float randomPercent = Random.Range(0, 1f);
        Vector3 startPos = new Vector3(Mathf.Lerp(startValue, endValue, randomPercent), slider.position.y, slider.position.z);

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
        float leftPos = hitArea.position.x - (hitArea.sizeDelta.x / 2);

        //get the rightmost pos
        float rightPos = hitArea.position.x + (hitArea.sizeDelta.x / 2);

        //compare the ends of the handle to the ends of the target position to check if the handle is within the bounds of the target area
        if (handle.position.x + handle.sizeDelta.x > leftPos && handle.position.x - handle.sizeDelta.x < rightPos)
        {
            onTarget = true;
        }

        return onTarget;
    }
}

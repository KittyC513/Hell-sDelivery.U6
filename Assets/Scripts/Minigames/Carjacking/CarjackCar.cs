using UnityEngine;

public class CarjackCar : MonoBehaviour
{
    private CarjackMinigameManager minigameManager;
    private CarUnlockSlider p1Slider;
    private CarUnlockSlider p2Slider;
    private CarUnlockSlider activeSlider;
    private bool active = false;
    private PlayerInputDetection currentPlayer;

    private bool canInput = true;

    private void Start()
    {
        minigameManager = FindFirstObjectByType<CarjackMinigameManager>();
        p1Slider = minigameManager.p1Slider;
        p2Slider = minigameManager.p2Slider;
    }

    public void StartUnlockMinigame(PlayerInputDetection playerInput)
    {
        if (!active)
        {
            CarUnlockSlider slider;
            active = true;

            if (playerInput.playerNum == 1) slider = p1Slider;
            else slider = p2Slider;

            //reset the slider minigame
            slider.ResetMinigame();
            slider.Activate(transform.position);

            //set the variables
            activeSlider = slider;
            currentPlayer = playerInput;
        }
       
    }

    private void Update()
    {
        if (active)
        {
            //read inputs for the active slider
            if (currentPlayer?.jumpPressed == true && canInput)
            {
                canInput = false;
                activeSlider?.CallInput();
            }


            //make sure they cant hold down the button
            if (currentPlayer?.jumpPressed != true)
            {
                if (canInput == false)
                {
                    canInput = true;
                }
            }

            if (!activeSlider.active)
            {
                //check if the game was completed
                if (activeSlider.completed == true)
                {
                    active = false;
                    currentPlayer = null;
                    activeSlider = null;
                }
                else
                {
                    //the game was failed
                    active = false;
                    currentPlayer = null;
                    activeSlider = null;
                }
            }
           
        }
    }
    
}
